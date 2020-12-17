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


            if (File.Exists(path))
            {
                pythonText = File.ReadAllLines(path);
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
                return lineIndex + 1;
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
                string temp = line.Replace("print(","");
                temp = temp.Remove(temp.LastIndexOf(")"));
                Console.WriteLine(removeWhiteSpaces(calculateValue(temp)));
            }
            // Check for variables
            else if (new Regex("\\s*[a-zA-Z_][a-zA-Z0-9_]* [-+*/^%]= .*").IsMatch(line))
            {
                return handleVariable(line) == 1 ? lineIndex + 1 : -1;
            }
            // Create variable
            else if (new Regex("\\s*[a-zA-Z_]+[a-zA-Z0-9_]* = .*").IsMatch(line))
            {
                return createVariable(line) == 1 ? lineIndex + 1 : -1;
            }

            return lineIndex+1;
        }

        private static int createVariable(string line)
        {
            string variable = "";
            string value = "";

            variable = line.Split("=")[0].Replace(" ", "");

            value = line.Split("=")[1];
            value = calculateValue(replaceVariables(value));
            if (variables.ContainsKey(variable))
            {
                variables[variable] = value;
            }
            else
            {

                variables.Add(variable, value);
            }
            return 1;
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
                value = variable + op + line.Split(op + "=")[1];
                value = calculateValue(replaceVariables(value));
                variables[variable] = value;
                return 1;
            }
            else
            {
                Console.WriteLine("Error: undeclared variable {0}.", variable);
                return -1;
            }
        }


        private static string calculateValue(string line)
        {
            line = replaceVariables(line);
            List<string> tokenList = new List<string>();
            bool nonOp = true;
            string num = "";
            while (line.Length > 0)
            {
                if (new Regex("^[0-9]").IsMatch(line))
                {
                    num += line[0].ToString();
                    line = line.Substring(1);
                    nonOp = false;
                    if(line.Length == 0)
                        tokenList.Add(num);
                }
                else if (new Regex("^[-+*/^%].*").IsMatch(line))
                {
                    if (line[0].Equals('-') && nonOp)
                    {
                        num += line[0].ToString();
                        line = line.Substring(1);
                        nonOp = false;
                    }
                    else
                    {
                        if (num != "")
                            tokenList.Add(num);
                        num = "";
                        tokenList.Add(line[0].ToString());
                        line = line.Substring(1);
                        nonOp = true;
                    }
                }
                else if (new Regex("^ .*").IsMatch(line))
                {
                    line = line.Substring(1);
                }
                else if (new Regex("^str\\(.*\\)").IsMatch(line))
                {
                    string temp = replaceVariables(line.Substring(4, line.IndexOf(")") - 4));
                    tokenList.Add(temp);
                    line = line.Substring(line.IndexOf(")") + 1);
                }
                else if (new Regex("\".*\"").IsMatch(line))
                {
                    string temp = line.Substring(0, 1);
                    line = line.Substring(1);
                    temp += line.Substring(0,line.IndexOf("\""));
                    tokenList.Add(temp);
                    
                    line = line.Substring(line.IndexOf("\"")+1);
                }
                else
                {
                    Console.WriteLine("Error in calculate value");
                }
            }
            

            while (tokenList.Count > 1)
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
                    try
                    {
                        double x = Double.Parse(tokenList[i - 1]);
                        double y = Double.Parse(tokenList[i + 1]);
                        x += y;
                        tokenList[i - 1] = x.ToString();
                        tokenList.RemoveRange(i, 2);
                    }
                    catch
                    {
                        string x = tokenList[i - 1];
                        string y = tokenList[i + 1];
                        x += y;
                        tokenList[i - 1] = x;
                        tokenList.RemoveRange(i, 2);
                    }
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

            // Make line the remaining condition by removing while and the colon
            if (new Regex("\\s*while.*:").IsMatch(line))
            {
                line = line.Replace("while ", "");
                line = line.Substring(0, line.LastIndexOf(":"));                
            }
            else
            {
                Console.WriteLine("Invalid while statement line {0}", lineIndex);
                return -1;
            }

            int i = 0, indentCount = 0;
            string line2 = pythonText[lineIndex];
            while (new Regex("^    .*").IsMatch(line2))
            {
                line2 = line2.Substring(4);
                indentCount++;
                i++;
            }


            string condition = removeWhiteSpaces(line);
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
            int forLoopIndent = 0;

            // Expect for keyword
            Match match = Regex.Match(line, "\\s*for ");
            if (match.Success)
            {
                forLoopIndent = match.Index;
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
            for (int i = 5; i < 25; i++)
            {
                bool isInForLoop = true;
                while (isInForLoop)
                {
                    lineIndex++;
                    line = pythonText[lineIndex];
                    match = Regex.Match(line, "\\s+[a-zA-Z_(),]+");
                    if (match.Success)
                    {
                        Match match2 = Regex.Match(line, "[a-zA-Z_(),#]+");
                        int currentIndent = match2.Index;
                        if (currentIndent >= forLoopIndent)
                        {
                            lineIndex = readline(pythonText, ++lineIndex);
                        }
                        else
                        {
                            isInForLoop = false;
                        }
                    }
                    else
                    {
                        isInForLoop = false;
                    }
                }

                lastIndex = lineIndex;
                lineIndex = startLineIndex;
            }

            return lastIndex;
        }

        private static int ifStatement(string[] pythonText, int lineIndex)
        {
            // IF Portion

            string line = pythonText[lineIndex];

            // Make line the remaining condition by removing while and the colon
            if (new Regex("\\s*if .*:").IsMatch(line))
            {
                line = line.Replace("if ", "");
                line = line.Substring(0, line.LastIndexOf(":"));
            }
            else if (new Regex("\\s*if\\(.*\\):").IsMatch(line))
            {
                line = line.Replace("if(", "");
                line = line.Substring(0, line.LastIndexOf(":") - 1);
            }
            else
            {
                Console.WriteLine("Invalid if statement line {0}", lineIndex);
                return -1;
            }

            int i = 0, indentCount = 0;
            string line2 = pythonText[lineIndex];
            while (new Regex("^    .*").IsMatch(line2))
            {
                line2 = line2.Substring(4);
                indentCount++;
                i++;
            }

            string condition = removeWhiteSpaces(line);
            int ifStart = lineIndex + 1;
            int ifEnd = findNextEqualLevel(pythonText, ifStart, indentCount);
            if (checkCondition(condition))
            {
                int ifIndex = ifStart;
                while (ifIndex < ifEnd && !pythonText[ifIndex].Equals(""))
                {
                    ifIndex = readline(pythonText, ifIndex);
                }
            }
            else
            {
                return elifStatement(pythonText, ifEnd);
            }

            while(new Regex("\\s*elif:.*:").IsMatch(pythonText[ifEnd]) || new Regex("\\s*else:").IsMatch(pythonText[ifEnd]))
            {
                ifEnd = findNextEqualLevel(pythonText, ifEnd, indentCount);
            }

            return ifEnd;
        }           

        private static int elifStatement(string[] pythonText, int lineIndex) 
        { 
            string line = pythonText[lineIndex];

            // Make line the remaining condition by removing while and the colon
            if (new Regex("\\s*elif .*:").IsMatch(line))
            {
                line = line.Replace("elif ", "");
                line = line.Substring(0, line.LastIndexOf(":"));
            }
            else if (new Regex("\\s*elif\\(.*\\):").IsMatch(line))
            {
                line = line.Replace("elif(", "");
                line = line.Substring(0, line.LastIndexOf(":") - 1);
            }
            else if (new Regex("\\s*else\\(.*\\):").IsMatch(line) || new Regex("\\s*else .*:").IsMatch(line))
            {
                return elseStatement(pythonText, lineIndex);
            }
            else
            {
                Console.WriteLine("Invalid if statement line {0}", lineIndex);
                return -1;
            }

            int i = 0, indentCount = 0;
            string line2 = pythonText[lineIndex];
            while (new Regex("^    .*").IsMatch(line2))
            {
                line2 = line2.Substring(4);
                indentCount++;
                i++;
            }

            string condition = removeWhiteSpaces(line);
            int elifStart = lineIndex + 1;
            int elifEnd = findNextEqualLevel(pythonText, elifStart, indentCount);
            if (checkCondition(condition))
            {
                int ifIndex = elifStart;
                while (ifIndex < elifEnd && !pythonText[ifIndex].Equals(""))
                {
                    ifIndex = readline(pythonText, ifIndex);
                }
            }
            else
            {
                return elifStatement(pythonText, elifEnd);
            }

            while (new Regex("\\s*elif:.*:").IsMatch(pythonText[elifEnd]) || new Regex("\\s*else:.*:").IsMatch(pythonText[elifEnd]))
            {
                elifEnd = findNextEqualLevel(pythonText, elifEnd, indentCount);
            }
            return elifEnd;
        }

        private static int elseStatement(string[] pythonText, int lineIndex)
        {
            string line = pythonText[lineIndex];

            // Make line the remaining condition by removing while and the colon
            if (!new Regex("\\s*else:").IsMatch(line))
            {
                Console.WriteLine("Invalid else statement line {0}", lineIndex);
                return -1;
            }

            int i = 0, indentCount = 0;
            string line2 = pythonText[lineIndex];
            while (new Regex("^    .*").IsMatch(line2))
            {
                line2 = line2.Substring(4);
                indentCount++;
                i++;
            }

            int ifEnd = findNextEqualLevel(pythonText, lineIndex + 1, indentCount);
            int ifIndex = lineIndex + 1;
            while (ifIndex < ifEnd && !pythonText[ifIndex].Equals(""))
            {
                ifIndex = readline(pythonText, ifIndex);
            }
            return ifEnd;
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
                // operand1 <= operand2
                case var condition when new Regex(digit + "<=" + digit).IsMatch(condition):
                    operand1 = Double.Parse(condition.Split("<=")[0]);
                    operand2 = Double.Parse(condition.Split("<=")[1]);
                    return operand1 <= operand2;
                // operand1 < operand2
                case var condition when new Regex(digit + "<" + digit).IsMatch(condition):
                    operand1 = Double.Parse(condition.Split("<")[0]);
                    operand2 = Double.Parse(condition.Split("<")[1]);
                    return operand1 < operand2;
                // operand1 <= operand2
                case var condition when new Regex(digit + ">=" + digit).IsMatch(condition):
                    operand1 = Double.Parse(condition.Split(">=")[0]);
                    operand2 = Double.Parse(condition.Split(">=")[1]);
                    return operand1 >= operand2;
                // operand1 > operand2
                case var condition when new Regex(digit + ">" + digit).IsMatch(condition):
                    operand1 = Double.Parse(removeWhiteSpaces( condition.Split(">")[0] ));
                    operand2 = Double.Parse(removeWhiteSpaces( condition.Split(">")[1] ));
                    return operand1 > operand2;
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
                    if (new Regex("\\s*print\\(.*\"" + variable.Key + "\".*\\)").IsMatch(statement) || new Regex("\\s*str\\(" + variable.Key + "\\)").IsMatch(statement))
                    {
                        // Skip
                        continue;
                    }
                    else
                    {
                        // Replace the variable name with its value
                        statement = statement.Replace(variable.Key, variable.Value);
                    }
                }
            }
            return statement;
        }

        private static string removeWhiteSpaces(string line)
        {
            while (new Regex("^ .*").IsMatch(line))
                line = line.Substring(1);
            while (new Regex(".* $").IsMatch(line))
                line = line.Substring(0, line.Length - 1);
            return line;
        }

        private static int findNextEqualLevel(string[] pythonText, int lineIndex, int level)
        {
            while (lineIndex < pythonText.Length)
            {
                int i = 0, indentCount = 0;
                string line = pythonText[lineIndex];
                while (new Regex("^    .*").IsMatch(line))
                {
                    line = line.Substring(4);
                    indentCount++;
                    i++;
                }
                if (indentCount == level)
                    return lineIndex;
                lineIndex++;
            }
            return -1;
        }
    }
}
