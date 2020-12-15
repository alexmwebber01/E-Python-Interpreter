
namespace proj
{
    public class Token<T>
    {
        // token type: INTEGER, STRING, EOF, ...
        public string type { get; set; }
        // token value, generic type
        public T value { get; set; }

        public Token(string _type, T _value)
        {
            type = _type;
            value = _value;
        }
    }
}