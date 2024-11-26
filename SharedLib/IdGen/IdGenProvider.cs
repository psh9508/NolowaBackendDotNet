using IdGen;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.IdGen
{
    public interface IIdGenProvider
    {
        long GenerateId();
    }

    public class IdGenProvider : IIdGenProvider
    {
        private readonly DateTime Epoch = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private byte _timestampBits = 49;
        private byte _generatorIDBits = 5;
        private byte _sequenceBits = 9;

        private readonly IdGenerator _idGenerator;

        public IdGenProvider()
        {
            var idStructure = new IdStructure(_timestampBits, _generatorIDBits, _sequenceBits);
            var options = new IdGeneratorOptions(idStructure, new DefaultTimeSource(Epoch), SequenceOverflowStrategy.SpinWait);

            _idGenerator = new IdGenerator(0, options); // 단일 서버로 생각하기 때문에 genid는 0
        }

        public long GenerateId()
        {
            return _idGenerator.CreateId();
        }
    }
}
