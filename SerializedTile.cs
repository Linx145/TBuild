using Terraria;
using Terraria.ID;

namespace TBuild
{
    internal struct SerializedTile
    {
        public int tileID;
        public byte tileColor;
        public bool HasTile => tileID > -1;
        public bool RedWire;
        public bool GreenWire;
        public bool YellowWire;
        public bool BlueWire;

        public short frameX;
        public short frameY;
        public byte wallColor;
        public SlopeType slopeType;

        public int liquidType;
        public byte liquidAmount;
        public ushort wallID;

        public bool hasActuator;
        public bool isActuated;

        public bool isHalfBlock;
    }
}
