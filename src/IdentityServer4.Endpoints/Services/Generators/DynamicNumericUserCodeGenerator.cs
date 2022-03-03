using IdentityServer4.Services;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace IdentityServer4.Anonymous.Services.Generators
{
    public class DynamicNumericUserCodeGenerator : IUserCodeGenerator
    {
        private readonly string _userCodeType;
        private readonly int _minValue;
        private readonly int _maxValue;

        public DynamicNumericUserCodeGenerator(int length, string userCodeType = null)
        {
            _userCodeType = userCodeType ?? (length + "FiguresNumeric");
            _minValue = 1;
            _maxValue = 9;
            var i = length - 1;
            while (i > 0)
            {
                _minValue *= 10;
                _maxValue *= 10;
                _maxValue += 9;
                i--;
            }
        }
        public string UserCodeType => _userCodeType;
        public int RetryLimit => 5;
        public Task<string> GenerateAsync() => Task.FromResult(Next().ToString());

        private int Next()
        {
            if (_minValue > _maxValue) throw new ArgumentOutOfRangeException(nameof(_minValue));
            if (_minValue == _maxValue) return _minValue;
            long diff = _maxValue - _minValue;

            var uint32Buffer = new byte[8];

            using var rng = new RNGCryptoServiceProvider();
            while (true)
            {
                rng.GetBytes(uint32Buffer);
                var rand = BitConverter.ToUInt32(uint32Buffer, 0);

                const long max = 1 + (long)uint.MaxValue;
                var remainder = max % diff;
                if (rand < max - remainder)
                {
                    return (int)(_minValue + rand % diff);
                }
            }
        }
    }
}
