#define DEBUG_MESSAGES

using System;

namespace UnityGLTF.Interactivity
{
    public ref struct StringSpanReader
    {
        private int _start;
        private int _end;
        private readonly ReadOnlySpan<char> _buffer;
        private readonly ReadOnlySpan<char> _avoidCharacters;


        public StringSpanReader(ReadOnlySpan<char> buffer, ReadOnlySpan<char> avoidCharacters)
        {
            _buffer = buffer;
            _avoidCharacters = avoidCharacters;
            _start = 0;
            _end = buffer.Length;
        }

        public bool GetFirstQuotedSubstring()
        {
            const char QUOTE = '\"';
            var startFound = false;
            var endFound = false;

            var start = 0;
            var end = 0;

            for (int i = _start; i < _end; i++)
            {
                if (_buffer[i] != QUOTE)
                    continue;

                start = i + 1;
                startFound = true;
                break;
            }

            if (!startFound)
                return false;

            for (int i = start; i < _end; i++)
            {
                if (_buffer[i] != QUOTE)
                    continue;

                end = i;
                endFound = true;
                break;
            }

            if (!endFound)
                return false;

            _start = start;
            _end = end;
            return true;
        }

        public bool FindFirstValidCharacter()
        {
            for (int i = _start; i < _buffer.Length; i++)
            {
                if (AnyMatch(_buffer[i], _avoidCharacters))
                    continue;

                _start = i;
                return true;
            }

            return false;
        }

        public void Slice()
        {
            for (int i = _start; i < _end; i++)
            {
                if (AnyMatch(_buffer[i], _avoidCharacters))
                    continue;
                
                _start = i;
                break;
            }

            for (int i = _start; i < _end; i++)
            {
                if (!AnyMatch(_buffer[i], _avoidCharacters))
                    continue;
                
                _end = i;
                break;
            }
        }

        public bool SetEndIndexByCharacters()
        {
            for (int i = _start; i < _buffer.Length; i++)
            {
                if (!AnyMatch(_buffer[i], _avoidCharacters))
                    continue;

                _end = i;
                return true;
            }

            return false;
        }

        public int CountCharacter(char c)
        {
            var count = 0;

            for (int i = _start; i < _end; i++)
            {
                if (_buffer[i] == c)
                    count++;
            }

            return count;
        }

        public ReadOnlySpan<char> AsReadOnlySpan()
        {
            return _buffer.Slice(_start, _end - _start);
        }

        public bool AnyMatch(char a, ReadOnlySpan<char> characters)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] == a)
                    return true;
            }

            return false;
        }

        public override string ToString()
        {
            return AsReadOnlySpan().ToString();
        }

        internal void SetStartIndexToEndIndex()
        {
            _start = _end;
        }
    }
}