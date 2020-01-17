using System;

namespace KoobecaFeedController.BL {
    public class RandomBytes {
        public static Random Rand = new Random();

        private static readonly byte[] Bytes = new byte[100000];
        private static bool _inited;
        private static readonly object Lock = new object();
        private static uint _index = 10000; // run between 10000 - 90000

        public static byte Next {
            get {
                if (!_inited)
                    lock (Lock) {
                        if (!_inited) {
                            Init();
                            _inited = true;
                        }
                    }

                var res = Bytes[_index++];
                if (_index > 90000) _index = 10000;

                return res;
            }
        }

        private static void Init() {
            var r = new Random();

            for (var i = 0; i < 100000; i++) Bytes[i] = (byte) r.Next(63);
        }
    }
}