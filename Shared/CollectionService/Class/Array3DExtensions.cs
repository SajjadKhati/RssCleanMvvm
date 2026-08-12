namespace Shared.CollectionService.Class
{
    public static class Array3DExtensions
    {

        public static byte[,,] To3DByteArray(this ushort[,,] source)
        {
            int y = source.GetLength(0);
            int x = source.GetLength(1);
            int channel = source.GetLength(2);
            var byteData = new byte[y, x, channel];

            for (int i = 0; i < y; i++)
                for (int j = 0; j < x; j++)
                    for (int k = 0; k < channel; k++)
                        byteData[i, j, k] = (byte)(source[i, j, k] >> 8);

            return byteData;
        }

    }
}
