using System.Reflection;

namespace sscpfe
{
    class OperationInfo
    {
        public int XPos { get; private set; }
        public int YPos { get; private set; }
        public int Repeats { get; private set; }
        public MethodInfo Method { get; private set; }
        public object[] Parametrs { get; private set; }

        public OperationInfo(int xPos, int yPos, int repeats, MethodInfo method, object[] parametrs)
        {
            XPos = xPos;
            YPos = yPos;
            Repeats = repeats;
            Method = method;
            Parametrs = parametrs;
        }
    }
}
