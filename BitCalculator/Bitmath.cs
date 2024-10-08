﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DevTools
{
    internal class Bitmath
    {
        public static string BitCalculate(string input)
        {
            var sINPUT = RemoveMultiplyDivide(RemoveBitShift(input));

            if (sINPUT.Contains('&') ||
                sINPUT.Contains('|') ||
                sINPUT.Contains('^') ||
                sINPUT.Contains('+') ||
                sINPUT.Contains('-'))
            {
                string firstUlong = "";
                int firstUlongStart_IDX = 0;
                bool firstTime = true;
                string secondUlong = "";
                char _operator = ' ';
                bool operatorSet = false;
                for (int i = 0; i < sINPUT.Length; i++)
                {
                    char c = sINPUT[i];
                    if (char.IsNumber(c))
                    {
                        //is it a number
                        if (!operatorSet)
                        {
                            firstUlong += c;
                            if (firstTime)
                            {
                                firstTime = false;
                                firstUlongStart_IDX = i;
                            }
                        }
                        else
                        {
                            secondUlong += c;
                        }
                    }
                    else if (c == '|' || c == '&' || c == '^' || c == '+' || c == '-')
                    {
                        if (secondUlong != "")
                        {
                            ulong result = 0ul;
                            if (firstUlong == "")
                            {
                                Program.modifyLastOutput = true; //We are modifying the previous input
                                firstUlong = Program.lastInput.ToString();
                                sINPUT = sINPUT.Insert(0, firstUlong);
                            }
                            switch (_operator)
                            {
                                case '&':
                                    result = ulong.Parse(firstUlong) & ulong.Parse(secondUlong);
                                    break;
                                case '|':
                                    result = ulong.Parse(firstUlong) | ulong.Parse(secondUlong);
                                    break;
                                case '^':
                                    result = ulong.Parse(firstUlong) ^ ulong.Parse(secondUlong);
                                    break;
                                case '+':
                                    result = ulong.Parse(firstUlong) + ulong.Parse(secondUlong);
                                    break;
                                case '-':
                                    result = ulong.Parse(firstUlong) - ulong.Parse(secondUlong);
                                    break;
                            }
                            //PrintColour(sINPUT + " = " + RemoveAndReplace(firstUlongStart_IDX, i, result.ToString(), sINPUT), true);
                            return BitCalculate(sINPUT.RemoveAndReplace(firstUlongStart_IDX, i, result.ToString()));
                        }
                        operatorSet = true;
                        _operator = c;
                    }
                    else
                    {
                        if (secondUlong != "")
                        {
                            ulong result = 0ul;
                            if (firstUlong == "")
                            {
                                Program.modifyLastOutput = true; //We are modifying the previous input
                                firstUlong = Program.lastInput.ToString();
                                sINPUT = sINPUT.Insert(0, firstUlong);
                                //firstUlongStart_IDX = 1;
                            }
                            switch (_operator)
                            {
                                case '&':
                                    result = ulong.Parse(firstUlong) & ulong.Parse(secondUlong);
                                    break;
                                case '|':
                                    result = ulong.Parse(firstUlong) | ulong.Parse(secondUlong);
                                    break;
                                case '^':
                                    result = ulong.Parse(firstUlong) ^ ulong.Parse(secondUlong);
                                    break;
                                case '+':
                                    result = ulong.Parse(firstUlong) + ulong.Parse(secondUlong);
                                    break;
                                case '-':
                                    result = ulong.Parse(firstUlong) - ulong.Parse(secondUlong);
                                    break;
                            }
                            //PrintColour(sINPUT + " = " + RemoveAndReplace(firstUlongStart_IDX, i, result.ToString(), sINPUT), true);
                            return BitCalculate(sINPUT.RemoveAndReplace(firstUlongStart_IDX, i, result.ToString()));
                        }
                        firstUlong = "";
                        operatorSet = false;
                        secondUlong = "";
                    }
                }
                if (secondUlong != "")
                {
                    ulong result = 0ul;
                    if (firstUlong == "")
                    {
                        Program.modifyLastOutput = true; //We are modifying the previous input
                        firstUlong = Program.lastInput.ToString();
                        sINPUT = sINPUT.Insert(0, firstUlong);
                        //firstUlongStart_IDX = 1;
                    }
                    ulong first = ulong.Parse(firstUlong);
                    ulong second = ulong.Parse(secondUlong);
                    switch (_operator)
                    {
                        case '&':
                            result = ulong.Parse(firstUlong) & ulong.Parse(secondUlong);
                            break;
                        case '|':
                            result = ulong.Parse(firstUlong) | ulong.Parse(secondUlong);
                            break;
                        case '^':
                            result = first ^ second;
                            break;
                        case '+':
                            result = ulong.Parse(firstUlong) + ulong.Parse(secondUlong);
                            break;
                        case '-':
                            result = ulong.Parse(firstUlong) - ulong.Parse(secondUlong);
                            break;
                    }
                    return BitCalculate(sINPUT.RemoveAndReplace(firstUlongStart_IDX, sINPUT.Length, result.ToString()));
                }
            }
            return sINPUT;
        }
        public static string DoubleCalculate(string input)
        {
            try
            {
                input = DoubleRemove_E(input);
                return Convert.ToDouble(new DataTable().Compute(input, null)).ExactDecimal();
            }
            catch
            {
                Program.expectingError = true;
                throw new Exception("Invalid equation");
            }
        }
        #region angleconversions
        public static double DegreeToRadian(double angle)
        {
            return angle * (Math.PI / 180.0);
        }
        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
        #endregion

        /// <summary>
        /// Removes trig statesments from the users input
        /// </summary>
        /// <param name="sINPUT"></param>
        /// <returns></returns>
        public static string RemoveTrig(string sINPUT)
        {
            if (sINPUT.Contains("sin(") ||
                sINPUT.Contains("arcsin(") ||
                sINPUT.Contains("tan(") ||
                sINPUT.Contains("arctan(") ||
                sINPUT.Contains("cos(") ||
                sINPUT.Contains("arccos("))
            {
                string buffer = "";
                for (int i = 0; i < sINPUT.Length; i++)
                {
                    char c = sINPUT[i];
                    buffer += c;
                    if (buffer.Contains("arcsin("))
                    {
                        return CalcTrig(sINPUT, i, MathAngleType.ArcSin);
                    }
                    if (buffer.Contains("arccos("))
                    {
                        return CalcTrig(sINPUT, i, MathAngleType.ArcCos);
                    }
                    if (buffer.Contains("arctan("))
                    {
                        return CalcTrig(sINPUT, i, MathAngleType.ArcTan);
                    }
                    if (buffer.Contains("sin("))
                    {
                        return CalcTrig(sINPUT, i, MathAngleType.Sin);
                    }
                    if (buffer.Contains("cos("))
                    {
                        return CalcTrig(sINPUT, i, MathAngleType.Cos);
                    }
                    if (buffer.Contains("tan("))
                    {
                        return CalcTrig(sINPUT, i, MathAngleType.Tan);
                    }
                }
            }
            return sINPUT;
        }
        enum MathAngleType
        {
            Sin,
            Cos,
            Tan,
            ArcSin,
            ArcCos,
            ArcTan,
        }
        private static string CalcTrig(string sINPUT, int stringIDX, MathAngleType mathAngleType)
        {
            string fixedval = sINPUT.Substring(0, stringIDX - 3); //Find the math that happens before it
            int nextOperaror = sINPUT.ClosingBracket(stringIDX + 1); //The next operator
            string result = DoubleCalculate(sINPUT.Substring(stringIDX + 1, nextOperaror - stringIDX - 1)); //Find the number between the brackets
            double calcNum = DegreeToRadian(double.Parse(result)); //Convert it from degrees to radians
            switch (mathAngleType)
            {
                case MathAngleType.Sin:
                    calcNum = Math.Sin(calcNum);
                    break;
                case MathAngleType.Cos:
                    calcNum = Math.Cos(calcNum);
                    break;
                case MathAngleType.Tan:
                    calcNum = Math.Tan(calcNum);
                    break;
                case MathAngleType.ArcSin:
                    calcNum = Math.Asin(calcNum);
                    break;
                case MathAngleType.ArcCos:
                    calcNum = Math.Acos(calcNum);
                    break;
                case MathAngleType.ArcTan:
                    calcNum = Math.Atan(calcNum);
                    break;
            }
            string afterThat = sINPUT.Substring(nextOperaror + 1, sINPUT.Length - nextOperaror - 1);
            CustomConsole.PrintColour(string.Format("{0}({1}) = {2}", mathAngleType.ToString(), result, calcNum));
            if (fixedval.EndsWith("arc")) //Fixed val has "arc" on the end of it? This happens for arccos, arcsin, and arctan operations
            {
                fixedval = fixedval.Substring(0,fixedval.Length-3);
            }
            string return_result = fixedval + calcNum + afterThat;

            if (return_result.StartsWith("np"))
            {
                Program.noprint = true;
                return_result = return_result.Substring(2);
            }
            if (return_result.Length <= 3 || return_result.Substring(0, 4) != "doum")
            {
                return_result = return_result.Insert(0, "doum");
            }
            return RemoveTrig(return_result);
        }

        public static string RemoveBrackets(string s)
        {
            string buffer = "";
            int firstBracketIDX = 0;
            bool addingToBuffer = false;
            for (int i = 0; i < s.Length; ++i)
            {
                var c = s[i];

                if (c == ')')
                {
                    string betweenBrackets = s.TextBetween(firstBracketIDX + 1, i - 1);
                    string total = BitCalculate(betweenBrackets);
                    string nextString = "";
                    for (int secondIDX = 0; secondIDX < s.Length; ++secondIDX)
                    {
                        if (secondIDX < firstBracketIDX || secondIDX > i)
                        {
                            nextString += s[secondIDX];
                        }
                        else if (secondIDX == firstBracketIDX)
                        {
                            nextString += total;
                        }
                    }
                    if (nextString.Contains('('))
                    {
                        return RemoveBrackets(nextString);
                    }
                    else
                    {
                        return nextString;
                    }
                }
                else if (addingToBuffer)
                {
                    buffer += c;
                }
                if (c == '(')
                {
                    if (addingToBuffer == true)
                    {
                        int nextBracketIDX = s.NextBracket(i);
                        if (nextBracketIDX == -1)
                        {
                            Program.expectingError = true;
                            throw new Exception("Amount of opening brackets inequal to amount of closing brackets");
                        }
                        if (s[nextBracketIDX] == ')') //Is this the last layer of brackets?
                        {
                            string betweenBrackets = s.TextBetween( i + 1, nextBracketIDX - 1);
                            string total = BitCalculate(betweenBrackets);
                            string nextString = "";
                            for (int secondIDX = 0; secondIDX < s.Length; ++secondIDX)
                            {
                                if (secondIDX < i || secondIDX > nextBracketIDX)
                                {
                                    nextString += s[secondIDX];
                                }
                                else if (secondIDX == i)
                                {
                                    nextString += total;
                                }
                            }
                            return RemoveBrackets(nextString);
                        }
                        else
                        {
                            firstBracketIDX = i;
                        }
                    }
                    else
                    {
                        addingToBuffer = true;
                        firstBracketIDX = i;
                    }
                }
            }
            return s;
        }

        public static double Average(string sINPUT)
        {
            sINPUT = sINPUT.Substring(4);
            sINPUT = sINPUT.Remove(sINPUT.Length - 1, 1);
            var snums = sINPUT.Split(',');
            List<double> doubles = new List<double>();
            foreach (var s in snums)
            {
                doubles.Add(double.Parse(s));
            }
            return doubles.Average();
        }
        private static string RemoveMultiplyDivide(string input)
        {
            if (input.Contains("*") || input.Contains("/"))
            {
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] == '/' || input[i] == '*')
                    {
                        int lastOperatorIDX = input.LastOperatorIDX(i - 2);
                        int nextOperatorIDX = input.NextOperatorIDX(i + 1);
                        string sub = input.Substring(lastOperatorIDX, (nextOperatorIDX - lastOperatorIDX));
                        string result = CalculateMultiplyDivide(sub);
                        string toReturn = string.Format("{0}{1}{2}", input.Substring(0, lastOperatorIDX == 0 ? 0 : lastOperatorIDX + 1), result, input.Substring(nextOperatorIDX, input.Length - nextOperatorIDX));
                        return RemoveMultiplyDivide(toReturn);
                    }
                }
            }
            return input;
        }
        private static string DoubleCalculateMultiplyDivide(string sINPUT)
        {
            if (sINPUT.Contains("*"))
            {
                double input;
                var strings = sINPUT.Split('*');
                if (strings[0] == "")
                {
                    if (Program.lastWasDouble)
                    {
                        strings[0] = BitConverter.Int64BitsToDouble((long)Program.lastInput).ExactDecimal(); //Convert bits to double
                    }
                    else
                    {
                        strings[0] = Program.lastInput.ToString();
                    }

                }
                double first = 0;
                double second = 0;
                double.TryParse(strings[1], out first);
                double.TryParse(strings[0], out second);
                if (strings[0][0] == '-')
                {
                    double.TryParse(strings[0].Substring(1), out second);
                    second = -second;
                }
                if (strings[1][0] == '-')
                {
                    double.TryParse(strings[1].Substring(1), out first);
                    first = -first;
                }
                input = second * first;
                //PrintColour(string.Format("{0} * {1} = {2}", second, first, input), true);
                return input.ExactDecimal();
            }
            else if (sINPUT.Contains("/"))
            {
                double input;
                var strings = sINPUT.Split('/');
                if (strings[0] == "")
                {
                    if (Program.lastWasDouble)
                    {
                        strings[0] = BitConverter.Int64BitsToDouble((long)Program.lastInput).ExactDecimal(); //Convert bits to double
                    }
                    else
                    {
                        strings[0] = Program.lastInput.ToString();
                    }
                }
                double first = 0;
                double second = 0;
                double.TryParse(strings[1], out first);
                double.TryParse(strings[0], out second);
                if (strings[0][0] == '-')
                {
                    double.TryParse(strings[0].Substring(1), out second);
                    second = -second;
                }
                if (strings[1][0] == '-')
                {
                    double.TryParse(strings[1].Substring(1), out first);
                    first = -first;
                }
                input = second / first;
                //PrintColour(string.Format("{0} / {1} = {2}", second, first, input), true);
                return input.ExactDecimal();
            }
            return "0";
        }
        public static string DoubleRemove_E(string input)
        {
            if (input.Contains("e-")) //Is there a 1.1E-1... number?
            {
                var middle = input.IndexOf("e-");
                var start = input.LastOperatorIDX(middle-1); //Look for last operator or letter. When we get there, we know that that is the index of the start of the number
                var end = input.NextOperatorIDX(middle+3); //Add 2 to ignore the E-

                string result = input.TextBetween(start,end);

                var before = input.Substring(0,start);
                var after = input.Substring(end);
                input = before + double.Parse(result).ExactDecimal() + after; //Convert double to an exact decimal, add it to the string
                return DoubleRemove_E(input); //Recursively try again
            }
            return input;
        }
        public static string RemoveLog(string userINPUT)
        {
            List<int> logidxs = userINPUT.AllIndexs("log"); //Find the positions of all the log statements
            var idx = userINPUT.IndexOf("log");
            if (idx == -1)
            {
                return userINPUT;
            }
            int openingBracketIDX = userINPUT.NextBracket(idx);

            var logbase = userINPUT.Substring(idx + 3, openingBracketIDX - idx - 3); //+3 is for the length of log
            if (logbase == "")
            {
                logbase = "10"; //Default base is 10
            }

            var lognum = userINPUT.Substring(openingBracketIDX + 1, userINPUT.ClosingBracket(openingBracketIDX + 1) - openingBracketIDX - 1);

            string before = userINPUT.Substring(0, idx); //Get the string that comes before this, up until idx
            string after = userINPUT.Substring(userINPUT.ClosingBracket(openingBracketIDX + 1) + 1); //End at the closing bracket
            string replace = Math.Log(double.Parse(lognum), double.Parse(logbase)).ToString();
            userINPUT = before + replace + after; //Modify the string

            userINPUT = RemoveLog(userINPUT);

            if (userINPUT.StartsWith("np")) //User doesn't want to print binary of the result
            {
                userINPUT = userINPUT.Substring(2);
                Program.noprint = true;
            }
            if (!userINPUT.StartsWith("doum"))
            {
                userINPUT = userINPUT.Insert(0, "doum");
            }

            return userINPUT;
        }
        /*
 * Calculates <<'s given a specific string
 */
        private static string CalculateBitShift(string sINPUT)
        {
            if (sINPUT.Contains("<<"))
            {
                ulong input;
                var strings = sINPUT.Split(new string[] { "<<" }, StringSplitOptions.None);
                if (strings[0] == "")
                {
                    strings[0] = "1";
                }
                var first = 0;
                var second = 0ul;
                int.TryParse(strings[1], out first);
                ulong.TryParse(strings[0], out second);
                input = (second << first);
                //CustomConsole.PrintColour(string.Format("{0} << {1} = {2}", second, first, input), true);
                return input.ToString();
            }
            else if (sINPUT.Contains(">>"))
            {
                ulong input;
                var strings = sINPUT.Split(new string[] { ">>" }, StringSplitOptions.None);
                if (strings[0] == "")
                {
                    Program.expectingError = true;
                    throw new Exception("For right shift operations you must specify a number");
                }
                var first = 0;
                var second = 0ul;
                int.TryParse(strings[1], out first);
                ulong.TryParse(strings[0], out second);
                input = (second >> first);
                //CustomConsole.PrintColour(string.Format("{0} >> {1} = {2}", second, first, input), true);
                return input.ToString();
            }
            return "0";
        }
        /// <summary>
        /// Like remove math, except exclusively for bitshifts
        /// </summary>
        /// <param name="input"></param>
        /// <param name="chosenType"></param>
        /// <returns></returns>
        public static string RemoveBitShift(string input)
        {
            string buffer = "";
            if (input.Contains("<<") || input.Contains(">>")) //Is there a bitwise operator?
            {
                for (int i = 0; i < input.Length; i++) //Iterate through the string
                {
                    buffer += input[i];
                    if (buffer.Contains("<<") || buffer.Contains(">>")) //Did we just get a <<?
                    {
                        buffer = ""; //Reset the buffer
                        int lastOperatorIDX = input.LastOperatorIDX(i - 2);
                        if ((lastOperatorIDX != 0 || input[0].IsOperator()) && i >= 2)
                        {
                            ++lastOperatorIDX;
                        }
                        int nextOperatorIDX = input.NextOperatorIDX(i + 1);
                        string sub = input.Substring(lastOperatorIDX, (nextOperatorIDX - lastOperatorIDX));
                        string result = CalculateBitShift(sub);
                        string toReturn = string.Format("{0}{1}{2}", input.Substring(0, lastOperatorIDX), result, input.Substring(nextOperatorIDX, input.Length - nextOperatorIDX));
                        return RemoveBitShift(toReturn);
                    }
                }
            }
            return input;
        }
        private static string CalculateMultiplyDivide(string sINPUT)
        {
            if (sINPUT.Contains("*"))
            {
                ulong input;
                var strings = sINPUT.Split('*');
                if (strings[0] == "")
                {
                    strings[0] = Program.lastInput.ToString();
                }
                var first = 0ul;
                var second = 0ul;
                ulong.TryParse(strings[1], out first);
                ulong.TryParse(strings[0], out second);
                input = second * first;
                //PrintColour(string.Format("{0} * {1} = {2}", second, first, input), true);
                return input.ToString();
            }
            else if (sINPUT.Contains("/"))
            {
                ulong input;
                var strings = sINPUT.Split('/');
                if (strings[0] == "")
                {
                    strings[0] = Program.lastInput.ToString();
                }
                var first = 0ul;
                var second = 0ul;
                ulong.TryParse(strings[1], out first);
                ulong.TryParse(strings[0], out second);
                if (first != 0)
                {
                    input = second / first;
                }
                else
                {
                    input = 0;
                }
                //PrintColour(string.Format("{0} / {1} = {2}", second, first, input), true);
                return input.ToString();
            }
            return "0";
        }

    }
}