using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode._2019
{
    public class IntCodeShell
    {
        IntCodeAsync _intCode;
        string _currentLine;
        int _currentIndex;

        TextReader _reader;
        TextWriter _writer;

        public IntCodeShell(IEnumerable<long> program, TextReader reader, TextWriter writer)
        {
            _intCode = new IntCodeAsync(
                program,
                Read,
                Write);

            _reader = reader;
            _writer = writer;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            await _intCode.RunAsync(cancellationToken);
        }

        private async Task<long> Read(CancellationToken cancellationToken)
        {
            if (_currentLine == null)
                _currentLine = await _reader.ReadLineAsync();

            if (_currentLine == null)
                throw new OperationCanceledException();

            if (_currentIndex == _currentLine.Length)
            {
                _currentIndex = 0;
                _currentLine = null;
                return '\n';
            }
            else
                return _currentLine[_currentIndex++];
        }

        private void Write(long value)
        {
            if (value >= 0 && value <= 127)
                _writer.Write((char)value);
            else
                _writer.Write(value.ToString());
        }

    }
}
