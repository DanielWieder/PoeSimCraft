using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Data
{
    public class PoeRandom : IRandom
    {
        readonly Random _random = new Random();
        public int Next()
        {
            return _random.Next();
        }

        public int Next(int max)
        {
            return _random.Next(max);
        }

        public int Next(int min, int max)
        {
            return _random.Next(min, max);
        }

        public double NextDouble()
        {
            return _random.NextDouble();
        }
    }

    public interface IRandom
    {
        int Next();
        int Next(int max);
        int Next(int min, int max);
        double NextDouble();
    }
}
