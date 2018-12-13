using System;
using System.IO;
using Joystick.Data;

namespace Joystick.Utils
{
    public static class Serializer
    {
        public static byte[] SerializeValuesResponse(DataResponse dataResponse)
        {
            var data = new MemoryStream();

            var xPosition = BitConverter.GetBytes(dataResponse.XPosition);
            data.Write(xPosition, 0, xPosition.Length);
            var yPosition = BitConverter.GetBytes(dataResponse.YPosition);
            data.Write(yPosition, 0, yPosition.Length);
            var isEnableLights = Convert.ToByte(dataResponse.IsEnableLights);
            data.Write(ConvertToByteArray(isEnableLights), 0, 1);

            return data.ToArray();
        }

        /// <summary>
        /// Конвертирует byte в массив байтов из одного элемента
        /// </summary>
        /// <param name="val">Байт</param>
        /// <returns></returns>
        private static byte[] ConvertToByteArray(byte val)
        {
            var bytes = new[] { val };
            return bytes;
        }
    }
}
