using System;
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


            if (File.Exists(path))
            {
                pythonText = File.ReadAllLines(path);
                //foreach (string text in pythonText)
                //{
                //    Console.WriteLine(text);
                //}
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
            if(lineIndex < 41)
            {
                return ++lineIndex;
                
            }
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
            else if (new Regex("\\s*[a-zA-Z_]+[a-zA-Z0-9_]* [-+*/^%]= .*").IsMatch(line))
            {
                return handleVariable(line) == 1 ? lineIndex + 1 : -1;
            }

            return lineIndex;
        }


        private static int handleVariable(string line)
        {
            string variable = "";
            string value = "";
            string op = "";
            switch (line)
            {
                case var condition when new Regex("\\s*[a-zA-Z0-9_]*\\s*-=.*").IsMatch(condition):
                    op = "-";
                    break;
                case var condition when new Regex("\\s*[a-zA-Z0-9_]*\\s*\\+=.*").IsMatch(condition):
                    op = "+";
                    break;
                case var condition when new Regex("\\s*[a-zA-Z0-9_]*\\s*\\*=.*").IsMatch(condition):
                    op = "*";
                    break;
                case var condition when new Regex("\\s*[a-zA-Z0-9_]*\\s*/=.*").IsMatch(condition):
                    op = "/";
                    break;
                case var condition when new Regex("\\s*[a-zA-Z0-9_]*\\s*\\^=.*").IsMatch(condition):
                    op = "^";
                    break;
                case var condition when new Regex("\\s*[a-zA-Z0-9_]*\\s*%=.*").IsMatch(condition):
                    op = "%";
                    break;
            }
            variable = line.Split(op + "=")[0].Replace(" ","");
            

            if (variables.ContainsKey(variable))
            {
                value = line.Split(op + "=")[1] + op + variable;
                value = calculateValue(replaceVariables(value));
                variables[variable] = value;
            }
            else
            {
                Console.WriteLine("Error: undeclared variable {0}.", variable);
            }

            if (!String.IsNullOrEmpty(variable))
            {
                // TODO: calculate value
                value = replaceVariables(value);


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


        private static string calculateValue(string line)
        {
            List<string> tokenList = new List<string>();
            int tokenCount = 0;
            while (line.Length > 1)
            {
                if(new Regex("^-?[0-9]*").IsMatch(line))
                {
                   if(tokenCount == 0 || new Regex("[-+*/^%]").IsMatch(tokenList[tokenCount-1]))
                   {
                        tokenList.Add(line.Substring(0, 2));
                        line = line.Substring(0, 2);
                   }
                   else
                   {
                        tokenList.Add(line.Substring(0,1));
                        line = line.Substring(0, 1);
                   }
                }
                else if (new Regex("^ .*").IsMatch(line))
                {
                    line = line.Substring(0, 1);
                }
            }

            while(tokenList.Count > 1)
            {
                if (tokenList.Count == 2)
                    return "-1";

                int i = tokenList.IndexOf("^");
                if (i != -1)
                {
                    double x = Double.Parse(tokenList[i - 1]);
                    double y = Double.Parse(tokenList[i + 1]);
                    x = Math.Pow(x, y);
                    tokenList[i - 1] = x.ToString();
                    tokenList.RemoveRange(i, 2);
                    continue;
                }
                i = tokenList.IndexOf("/");
                if (i != -1)
                {
                    double x = Double.Parse(tokenList[i - 1]);
                    double y = Double.Parse(tokenList[i + 1]);
                    x /= y;
                    tokenList[i - 1] = x.ToString();
                    tokenList.RemoveRange(i, 2);
                    continue;
                }
                i = tokenList.IndexOf("%");
                if (i != -1)
                {
                    double x = Double.Parse(tokenList[i - 1]);
                    double y = Double.Parse(tokenList[i + 1]);
                    x %= y;
                    tokenList[i - 1] = x.ToString();
                    tokenList.RemoveRange(i, 2);
                    continue;
                }
                i = tokenList.IndexOf("*");
                if (i != -1)
                {
                    double x = Double.Parse(tokenList[i - 1]);
                    double y = Double.Parse(tokenList[i + 1]);
                    x *= y;
                    tokenList[i - 1] = x.ToString();
                    tokenList.RemoveRange(i, 2);
                    continue;
                }
                i = tokenList.IndexOf("+");
                if (i != -1)
                {
                    double x = Double.Parse(tokenList[i - 1]);
                    double y = Double.Parse(tokenList[i + 1]);
                    x += y;
                    tokenList[i - 1] = x.ToString();
                    tokenList.RemoveRange(i, 2);
                    continue;
                }
                i = tokenList.IndexOf("-");
                if (i != -1)
                {
                    double x = Double.Parse(tokenList[i - 1]);
                    double y = Double.Parse(tokenList[i + 1]);
                    x -= y;
                    tokenList[i - 1] = x.ToString();
                    tokenList.RemoveRange(i, 2);
                    continue;
                }
            }

            return tokenList[0];
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
            int startLineIndex = lineIndex;
            string line = pythonText[startLineIndex];
            var localVariable = "";
            var iterable = "";


            // Expect for keyword
            Match match = Regex.Match(line, "\\s*for ");
            if (match.Success)
            {
                line = line.Remove(0, match.Index + match.Length);
            }
            else
            {
                Console.WriteLine("Expected for keyword!");
                return -1;
            }

            // Create local variable for iterable
            match = Regex.Match(line, "[a-zA-Z_]+[a-zA-Z0-9_]*");
            if (match.Success)
            {
                localVariable = line.Substring(0, match.Index + match.Length);
                line = line.Remove(0, match.Index + match.Length);
            }
            else
            {
                Console.WriteLine("Expected a forloop variable!");
                return -1;
            }


            // Expect in keyword
            match = Regex.Match(line, "\\s*in ");
            if (match.Success)
            {
                line = line.Remove(0, match.Index + match.Length);
            }
            else
            {
                Console.WriteLine("Expected in keyword!");
                return -1;
            }

            // Create Iterable
            match = Regex.Match(line, "[a-zA-Z_(), ]+");
            if (match.Success)
            {
                iterable = line.Substring(0, match.Index + match.Length);
                line = line.Remove(0, match.Index + match.Length);
            }
            else
            {
                Console.WriteLine("Expected interable!");
                return -1;
            }

            // Check for colon
            match = Regex.Match(line, ":\\s*");
            if (match.Success)
            {
                line = line.Remove(0, match.Index + match.Length);
            }
            else
            {
                Console.WriteLine("Expected interable!");
                return -1;
            }


            //TODO Abstract range to token
            int lastIndex = startLineIndex;
            for(int i = 5; i < 25; i++)
            {
                readline(pythonText, ++lineIndex);
                lastIndex = lineIndex;
                lineIndex = startLineIndex;
            }

            return lastIndex;
        }

        /*
        private static string getForLoopVariable(string line)
        {
            // Running out of time...
            string forLoopString = "for ";
            string inKeyWord = " in ";
            int start = line.IndexOf(forLoopString);
            if (start < 0)
            {
                Console.WriteLine("Missing key word \"for\"");
                return null;
            }
            
            start = start + forLoopString.Length;

            int end = line.IndexOf(inKeyWord);
            if (end < 0)
            {
                Console.WriteLine("Missing key word \"in\"");
            }

            string key = line.Substring(start, end - start);

            return key;
        }*/

        /*private static string getForLoopInterable(string line)
        {
            // Running out of time...
            string inKeyWord = " in ";
            int start = line.IndexOf(inKeyWord);
            if (start < 0)
            {
                Console.WriteLine("Missing key word \"in\"");
            }

            //string key = line.Substring(start + inKeyWord.Length, end);

            return null;
        }*/


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
