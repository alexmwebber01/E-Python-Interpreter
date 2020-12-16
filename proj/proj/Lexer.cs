using System;
using Token.cs;

namespace proj
{
    // The Lexer takes the raw input as a string and turns it
    // into tokens 
    public class Lexer
    {
        // input text (taken from file)
        public string text { get; set; }
        // index of the position in text
        public int pos { get; set; }
        // this will be text[pos]
        public char current_char { get; set; }
        public bool end_of_file { get; set; }

        // constructor
        public Lexer(string _text)
        {
            text = _text;
            pos = 0;
            current_char = text[0];
            end_of_file = false;
        }

        // advances the current_char
        public void advance()
        {
            pos++;
            if (pos > text.Length - 1)
                end_of_file = true; // end of input text (EOF)
            else
                current_char = text[pos];
        }

        // skips over any whitespaces (may or may not be useful)
        public void skip_whitespace()
        {
            while (!end_of_file && current_char == ' ')
                advance();
        }

        // returns an integer from the text
        // TODO : check if exceeding bounds of int
        public string get_integer()
        {
            var result = "";
            while (!end_of_file && Char.IsNumber(current_char))
            {
                result += current_char;
                advance();
            }
            return result;
        }

        // breaks text into tokens one at a time
        public Token get_token()
        {
            while (!end_of_file)
            {
                if (Char.IsNumber(current_char)) {
                    return new Token("INTEGER", get_integer());
                }

                if (current_char == '+') {
                    advance();
                    return new Token("PLUS", "+");
                }

                if (current_char == '-') {
                    advance();
                    return new Token("MINUS", "-");
                }

                if (current_char == '*') {
                    advance();
                    return new Token("MULT", "*");
                }

                if (current_char == '/') {
                    advance();
                    return new Token("DIV", "/");
                }

                if (current_char == '(') {
                    advance();
                    return new Token("LPAREN", "(");
                }

                if (current_char == ')') {
                    advance();
                    return new Token("RPAREN", ")");
                }
            }
            return new Token("EOF", null);
        }
    }
}