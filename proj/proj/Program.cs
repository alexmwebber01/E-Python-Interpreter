﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace proj
{
    class Program
    {
        private static IDictionary<string, string> variables = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            // TODO: allow for args?
            string path = "../../../test/python_test_code.py";
            string[] pythonText;

            variables.Add("name", "Ash Ketchum");

            variables.Add("charmender_HP", "110");
            variables.Add("squirtle_HP", "125");
            variables.Add("bulbasaur_HP", "150");

            variables.Add("charmender_attack", "40");
            variables.Add("squirtle_attack", "35");
            variables.Add("bulbasaur_attack", "25");

            //List<Variable> localVariables = new List<Variable>();
            if (File.Exists(path))
            {
                pythonText = File.ReadAllLines(path);
                foreach (string text in pythonText)
                {
                    Console.WriteLine(text);
                }
            }
            else
            {
                Console.WriteLine("File not found.");
                return;
            }

            int lineIndex = 0;
            while (lineIndex < pythonText.Length)
            {
                lineIndex = readline(pythonText, lineIndex);
                if(lineIndex == -1)
                {
                    break;
                }
            }
        }

        private static int readline(string[] pythonText, int lineIndex)
        {
            string line = pythonText[lineIndex];
            // Check for comment and blank spaces
            if(new Regex("(^\\s*#.*)|(^\\s*$)").IsMatch(line))
            {
                lineIndex++;
            }
            // Check for while loop
            else if (new Regex("^\\s*while.*").IsMatch(line))
            {
                return whileLoop(pythonText, lineIndex);
            }
            // Check for for loop
            else if (new Regex("^\\s*for.*").IsMatch(line))
            {
                return forLoop(pythonText, lineIndex);
            }
            // Check for if statement
            else if (new Regex("^\\s*if.*").IsMatch(line))
            {
                return ifStatement(pythonText, lineIndex);
            }
            // Check for print statement
            else if (new Regex("\\s*print\\(.*\\).*").IsMatch(line))
            {
                Console.WriteLine(line);
            }
            // Check for variables
            else if (new Regex("\\s*[a-zA-Z_]+[a-zA-Z0-9_]* [-+*/^%]?= .*").IsMatch(line))
            {
                return handleVariable(line) == 1 ? lineIndex + 1 : -1;
            }

            return lineIndex;
        }


        private static int handleVariable(string line)
        {
            string variable = "";
            string value = "";
            switch (line)
            {
                case var condition when new Regex("\\s*[a-zA-Z0-9_]* =.*").IsMatch(condition):
                    variable = condition.Split("=")[0];
                    value = condition.Split("=")[1];
                    break;
                case var condition when new Regex("\\s*[a-zA-Z0-9_]* -=.*").IsMatch(condition):
                    variable = condition.Split("-=")[0];
                    value = condition.Split("-=")[1];
                    break;
                case var condition when new Regex("\\s*[a-zA-Z0-9_]* \\+=.*").IsMatch(condition):
                    variable = condition.Split("+=")[0];
                    value = condition.Split("+=")[1];
                    break;
                case var condition when new Regex("\\s*[a-zA-Z0-9_]* \\*=.*").IsMatch(condition):
                    variable = condition.Split("*=")[0];
                    value = condition.Split("*=")[1];
                    break;
                case var condition when new Regex("\\s*[a-zA-Z0-9_]* \\=.*").IsMatch(condition):
                    variable = condition.Split("\\=")[0];
                    value = condition.Split("\\=")[1];
                    break;
                case var condition when new Regex("\\s*[a-zA-Z0-9_]* \\^=.*").IsMatch(condition):
                    variable = condition.Split("^=")[0];
                    value = condition.Split("^=")[1];
                    break;
                case var condition when new Regex("\\s*[a-zA-Z0-9_]* %=.*").IsMatch(condition):
                    variable = condition.Split("%=")[0].Replace(" ","");
                    value = condition.Split("%=")[1].Replace(" ","");
                    break;
            }
            if (!String.IsNullOrEmpty(variable))
            {
                // TODO: calculate value

                foreach (KeyValuePair<string, string> item in variables)
                {
                    if (variable.Contains(item.Key))
                    {
                        if(getVariableType(item.Value) == getVariableType(value))
                        {
                            variables[item.Key] = value;
                        }
                        else
                        {
                            Console.WriteLine("Error: variable of type {0} cannot not be assigned to type {1}.", getVariableType(item.Value), getVariableType(value));
                            return -1;
                        }
                    }
                }
                variables.Add(variable, value);
                return 1;
            }
            else
            {
                return -1;
            }
        }
        
        private static string getVariableType(string variable)
        {
            if (new Regex("\".*\"").IsMatch(variable))
            {
                return "string";
            }
            else if (new Regex("(true|false)").IsMatch(variable))
            {
                return "bool";
            }
            else if (new Regex("\\d*").IsMatch(variable))
            {
                return "double";
            }
            return "-1";
        }

        private static int whileLoop(string[] pythonText, int lineIndex)
        {
            string line = pythonText[lineIndex];
            // Remove white space at beginning of line
            line = removeLeadingWhiteSpaces(line);

            // Make line the remaining condition by removing while and the colon
            if (new Regex("\\s*while.*:").IsMatch(line))
            {
                line = line.Substring(line.IndexOf("while "), line.LastIndexOf(":"));
            }
            else
            {
                Console.WriteLine("Invalid while statement line {0}", lineIndex);
                return -1;
            }

            int i = 0, indentCount = 0;
            while (line[i].Equals(" "))
            {
                indentCount++;
                i++;
            }
            indentCount /= 4;

            string condition = removeLeadingWhiteSpaces(line);
            int whileLoopStart = lineIndex + 1;
            int whileLoopEnd = findNextEqualLevel(pythonText,whileLoopStart,indentCount);
            while (checkCondition(condition))
            {
                int whileLoopIndex = whileLoopStart;
                while(whileLoopIndex < whileLoopEnd && !pythonText[whileLoopIndex].Equals(""))
                {
                    whileLoopIndex = readline(pythonText, whileLoopIndex);
                }
            }

            return whileLoopEnd;
        }

        private static int forLoop(string[] pythonText, int lineIndex)
        {
            string line = pythonText[lineIndex];
            int i = 0, indentCount = 0;
            while (line[i].Equals(" "))
            {
                indentCount++;
                i++;
            }
            indentCount /= 4;
            return findNextEqualLevel(pythonText, lineIndex+1, indentCount);
        }

        private static int ifStatement(string[] pythonText, int lineIndex)
        {
            string line = pythonText[lineIndex];
            int i = 0, indentCount = 0;
            while (line[i].Equals(" "))
            {
                indentCount++;
                i++;
            }
            indentCount /= 4;
            lineIndex = findNextEqualLevel(pythonText, lineIndex + 1, indentCount);
            return findNextEqualLevel(pythonText, lineIndex + 1, indentCount);
        }

        private static bool checkCondition(string conditionString)
        {
            // Replace variables in condition string with raw values
            conditionString = replaceVariables(conditionString);
            string[] conditionStatements;

            // check for multiple conditions using and
            if (conditionString.Contains(" and "))
            {
                conditionStatements = conditionString.Split(" and ");
                foreach (string condition in conditionStatements)
                {
                    if (!checkCondition(condition))
                        return false;
                }
                return true;
            }

            // check for multiple conditions using or
            if (conditionString.Contains(" or "))
            {
                conditionStatements = conditionString.Split(" or ");
                foreach (string condition in conditionStatements)
                {
                    if (checkCondition(condition))
                        return true;
                }
                return false;
            }

            // Remove all spaces
            conditionString = conditionString.Replace(" ", "");

            // Check for conditional operator and use to split conditionString
            // store right operand in operand1 and left operand in operand 2
            // return result based on conditional operator
            string digit = "\\d*.*\\d*";
            double operand1, operand2;
            switch (conditionString)
            {
                // operand1 < operand2
                case var condition when new Regex(digit + "<" + digit).IsMatch(condition):
                    operand1 = Double.Parse(condition.Split("<")[0]);
                    operand2 = Double.Parse(condition.Split("<")[1]);
                    return operand1 < operand2;
                // operand1 <= operand2
                case var condition when new Regex(digit + "<=" + digit).IsMatch(condition):
                    operand1 = Double.Parse(condition.Split("<=")[0]);
                    operand2 = Double.Parse(condition.Split("<=")[1]);
                    return operand1 <= operand2;
                // operand1 > operand2
                case var condition when new Regex(digit + ">" + digit).IsMatch(condition):
                    operand1 = Double.Parse(condition.Split(">")[0]);
                    operand2 = Double.Parse(condition.Split(">")[1]);
                    return operand1 < operand2;
                // operand1 <= operand2
                case var condition when new Regex(digit + ">=" + digit).IsMatch(condition):
                    operand1 = Double.Parse(condition.Split(">=")[0]);
                    operand2 = Double.Parse(condition.Split(">=")[1]);
                    return operand1 <= operand2;
                // operand1 == operand2
                case var condition when new Regex(digit + "==" + digit).IsMatch(condition):
                    operand1 = Double.Parse(condition.Split("==")[0]);
                    operand2 = Double.Parse(condition.Split("==")[1]);
                    return operand1 == operand2;
                // operand1 != operand2
                case var condition when new Regex(digit + "!=" + digit).IsMatch(condition):
                    operand1 = Double.Parse(condition.Split("!=")[0]);
                    operand2 = Double.Parse(condition.Split("!=")[1]);
                    return operand1 != operand2;
                default:
                    return false;
            }
        }

        private static string replaceVariables(string statement)
        {
            // statement to replace all variable names with their values
            foreach (KeyValuePair<string, string> variable in variables)
            {
                if (statement.Contains(variable.Key))
                {
                    // Check if the variable was use inside quoatation marks
                    if (new Regex("\\s*print\\(.*\"" + variable.Key + "\".*\\)").IsMatch(statement))
                    {
                        // Skip
                        continue;
                    }
                    else
                    {
                        // Replace the variable name with its value
                        statement.Replace("\\b" + variable.Key + "\\b", variable.Value);
                    }
                }
            }
            return statement;
        }

        private static string removeLeadingWhiteSpaces(string line)
        {
            while (new Regex("^ .*").IsMatch(line))
                line = line.Substring(1);
            return line;
        }

        private static int findNextEqualLevel(string[] pythonText, int lineIndex, int level)
        {
            while (lineIndex < pythonText.Length)
            {
                int i = 0, indentCount = 0;
                while (pythonText[lineIndex][i].Equals(" "))
                {
                    indentCount++;
                    i++;
                }
                indentCount /= 4;
                if (indentCount == level)
                    return lineIndex;
                lineIndex++;
            }
            return -1;
        }
    }
}
