
namespace proj
{
    public class Token
    {
        // token type: INTEGER, STRING, EOF, ...
        public string type { get; set; }
        // token value, e.g. "24" for an int "alsjdf" for a string
        // will have to typecast this to get the actual value according to the type
        public string value { get; set; }

        public Token(string _type, string _value)
        {
            type = _type;
            value = _value;
        }
    }
}