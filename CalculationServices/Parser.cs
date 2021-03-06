﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CalculationServices.ParserExcepton;

namespace CalculationServices.Parser
{
    /// <summary>
    /// Parser return this class's instance.
    /// </summary>
    public class ParserResult
    {
        #region FIELDS
        /// <summary>
        /// 計算結果の数値
        /// </summary>
        private long result= 0;
        /// <summary>
        /// 計算結果の正常・異常メッセージを表現する文字列。
        /// </summary>
        private string message = null;

        #endregion

        #region PROPERTY
        /// <summary>
        /// get result of calculation.
        /// </summary>
        public long Result
        {
            get { return result; }
        }
        /// <summary>
        /// get message of calculation. 
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        #endregion 

        #region METHODS

        /// <summary>
        /// Default constructor
        /// </summary>
        public ParserResult()
        {
        }
        /// <summary>
        /// Constructor.
        /// Only set 'result'.
        /// </summary>
        /// <param name="result">result valus's string</param>
        public ParserResult(long result)
        {
            this.result = result;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        public ParserResult(long result , string message)
        {
            this.result = result;
            this.message = message;
        }
        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="obj">Instance for copied source.</param>
        public ParserResult(ParserResult obj)
        {
            this.result = obj.result;
            this.message = obj.message;
        }
        #endregion
    }
    public class Messages
    {
        public const string MSG_SUCCESS = "";
        public const string MSG_NULL = "Statement is not allowed null.";
        public const string MSG_EMPTY = "Statement is not allowed empty.";
    }

    public class ParsedData
    {
        /// <summary>
        /// 参照中のインデックスを保持する
        /// </summary>
        private int index = 0;
        /// <summary>
        /// メッセージがあれば保持する
        /// </summary>
        private string message = null;
        /// <summary>
        /// 計算途中の値を保持する
        /// </summary>
        private long number = 0;
        /// <summary>
        /// PROPTERY : 
        /// </summary>
        public int INDEX
        {
            get { return index; }
            set { this.index = value; }
        }
        /// <summary>
        /// PROPTERY : 
        /// Get innternal message.
        /// </summary>
        public string MESSAGE
        {
            get { return message; }
        }
        /// <summary>
        /// PROPTERY : 
        /// Get Calculated value.
        /// </summary>
        public long NUMBER
        {
            get { return number; }
            set { this.number = value; }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="num"></param>
        /// <param name="idx"></param>
        /// <param name="msg"></param>
        public ParsedData(long num, int idx, string msg)
        {
            this.number = num;
            this.index = idx;
            this.message = msg;
        }

    }



    /// <summary>
    /// Parse calculation's statement and output the result of calculation.
    /// 計算式の構文解析クラスの外部インターフェースクラス
    /// </summary>
    public partial class Parser
    {
        #region FIELDS
        #endregion

        #region PROPERTY
        #endregion

        #region METHODS
        /// <summary>
        /// Constructor
        /// </summary>
        public Parser()
        {
        }

        /// <summary>
        /// 計算機の入力式を解析して結果を返す.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns>Success : 計算結果(数値)の文字列を返す, Failure : 失敗した際はエラー結果の文字列を返す。</returns>
        public ParserResult parse(string statement)
        {
            if (statement == null)
                return new ParserResult(0, Messages.MSG_NULL);
            else if (statement.Equals(""))
                return new ParserResult(0, Messages.MSG_EMPTY);

            try
            {
                return parse_exec(statement);
            }catch(SyntaxException ex)
            {
                return new ParserResult(0, ex.Message);
            }
            catch(ParseOverflowException ex)
            {
                return new ParserResult(0, string.Format("{0}文字目からの入力文字{1}で数値の上下限値を超えました。", ex.INDEX, ex.EXCEPTION_DATA));
            }
            catch(Exception ex)
            {
                return new ParserResult(0, ex.Message);
            }
        }
        /// <summary>
        /// 計算機の入力式を解析する実行部
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        private ParserResult parse_exec( string statement)
        {
            var data = new ParsedData(0, 0, "");
            var result = expr(statement,data);

            // Check Syntax Accept Condition
            if (isExit(statement, result))
            {
                return new ParserResult(result.NUMBER, result.MESSAGE);
            }
            else
            {
                //statementの終端に達する前に解析終了しました。
                //文法エラーが発生しています。
                throw new SyntaxException(string.Format("{0}文字目近辺で異常終了しました。",result.INDEX),result.INDEX);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statement">input expression's statement</param>
        /// <param name="data"></param>
        /// <returns></returns>
        private ParsedData expr(string statement , ParsedData data)
        {
            var rLeft = term(statement , data);
            if (isExit(statement, rLeft))
            {
                return rLeft;
            }

            while (true)
            {
                var tokenCurrent = statement.ElementAt(rLeft.INDEX);
                if (tokenCurrent == Token.Operator.ADD)
                {
                    var rRight = term(statement, new ParsedData(rLeft.NUMBER, rLeft.INDEX + 1, rLeft.MESSAGE));
                    rLeft.INDEX = rRight.INDEX;
                    rLeft.NUMBER = rLeft.NUMBER + rRight.NUMBER;
                }
                else if (tokenCurrent == Token.Operator.SUB)
                {
                    var rRight = term(statement, new ParsedData(rLeft.NUMBER, rLeft.INDEX + 1, rLeft.MESSAGE));
                    rLeft.INDEX = rRight.INDEX;
                    rLeft.NUMBER = rLeft.NUMBER - rRight.NUMBER;
                }
                else
                {
                    break;
                }
                if (isExit(statement, rLeft))
                    break;
            }
            return rLeft;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statement">input expression's statement</param>
        /// <param name="data"></param>
        /// <returns></returns>
        private ParsedData term(string statement, ParsedData data)
        {
            var rLeft = factor(statement, data);

            if (isExit(statement, rLeft))
            {
                return rLeft;
            }
            while (true)
            {
                var token = statement.ElementAt(rLeft.INDEX);
                if (token == Token.Operator.MUL)
                {
                    var rRight = factor(statement, new ParsedData(rLeft.NUMBER, rLeft.INDEX + 1, rLeft.MESSAGE));
                    rLeft.INDEX = rRight.INDEX;
                    rLeft.NUMBER = rLeft.NUMBER * rRight.NUMBER;
                }
                else if (token == Token.Operator.DIV)
                {
                    var rRight = factor(statement, new ParsedData(rLeft.NUMBER, rLeft.INDEX + 1, rLeft.MESSAGE));
                    rLeft.INDEX = rRight.INDEX;
                    rLeft.NUMBER = rLeft.NUMBER / rRight.NUMBER;
                }
                else
                {
                    break;
                }
                if (isExit(statement, rLeft))
                    break;
            }
            return rLeft;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statement">input expression's statement</param>
        /// <param name="data"></param>
        /// <returns></returns>
        private ParsedData factor(string statement, ParsedData data)
        {
            var token = statement.ElementAt(data.INDEX);
            if(token == Token.Parenthesis.BEGIN )
            {
                data.INDEX++;

                if(isExit(statement,data))
                {
                    throw new SyntaxException("'('と')'の対応が合いません。",data.INDEX);
                }

                var rExpr = expr(statement, data);
                if (isExit(statement, rExpr))
                {
                    throw new SyntaxException(")の対応が合わず、終端に到達しました。",data.INDEX);
                }
                if (statement.ElementAt(rExpr.INDEX) != Token.Parenthesis.END)
                {
                    throw new SyntaxException(")が合わない",data.INDEX);
                }
                rExpr.INDEX++;

                return rExpr;
            }
            else 
            {
                string strNumber = "";

                if (isMinusSign(token))
                {
                    data.INDEX = data.INDEX + 1;
                    token = statement.ElementAt(data.INDEX);

                    strNumber = Token.Num.MINUS.ToString();
                }

                // 数字が一文字以上連続している場合は、数字として認識する。
                if (!isNumber(token))
                    throw new SyntaxException("????",data.INDEX);

                strNumber += token.ToString();

                var idxBegin = data.INDEX;
                var idxEnd = idxBegin;

                for (int idx = idxBegin + 1; idx < statement.Length; idx++ )
                {
                    var numWrk = statement.ElementAt(idxEnd + 1);
                    if (!isNumber(numWrk))
                        break;
                    idxEnd++;
                    strNumber += numWrk.ToString();
                }

                try
                {
                    var number = long.Parse(strNumber);
                    idxEnd++;
                    return new ParsedData(number, idxEnd, data.MESSAGE);
                }
                catch(OverflowException ex)
                {
                    throw new ParseOverflowException(ex.Message, idxBegin, strNumber);
                }
            }
        }
        /// <summary>
        /// Check if argument's value is number or not.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>true : number , false : not number</returns>
        private bool isNumber(char input)
        {
            return Token.isNumber(input);
        }
        /// <summary>
        /// Check if input character is minus sign or not.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>true : minus sign , false : not minus sign</returns>
        private bool isMinusSign(char input)
        {
            return Token.isMinusSign(input);
        }
        /// <summary>
        /// Check if input character is operator's sign.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool isOperator(char input)
        {
            return Token.isOperator(input);
        }
        /// <summary>
        /// Check if input character is parenthesis.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>true:parenthesis , false:not parenthesis</returns>
        private bool isParenthesis(char input)
        {
            return Token.isParenthesis(input);
        }
        /// <summary>
        /// Check if input character is able to accept for parser.
        /// 入力文字が解析器が受付可能な文字かチェックする
        /// </summary>
        /// <param name="input"></param>
        /// <returns>true :acceptable , false :not acceptable</returns>
        private bool isToken(char input)
        {
            return Token.isToken(input);
        }

        /// <summary>
        /// Check if parse point reach end point or not.
        /// </summary>
        /// <param name="statement">input</param>
        /// <param name="data"></param>
        /// <returns>true:reach end point , false:not reach end point.</returns>
        private bool isExit(string statement , ParsedData data)
        {
            return statement.Length <= data.INDEX;
        }
        #endregion
    }
}
