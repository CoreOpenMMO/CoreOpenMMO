namespace COTS.GameServer.Items
{
    public enum OTBAttributes : byte
    {
        ROOT_VERSION = 0x01,
        ITEM_SERVERID = 0x10,
        ITEM_CLIENTID,
        ITEM_NAME,
        ITEM_DESCR,
        ITEM_SPEED,
        ITEM_SLOT,
        ITEM_MAXITEMS,
        ITEM_WEIGHT,
        ITEM_WEAPON,
        ITEM_AMU,
        ITEM_ARMOR,
        ITEM_MAGLEVEL,
        ITEM_MAGFIELDTYPE,
        ITEM_WRITEABLE,
        ITEM_ROTATETO,
        ITEM_DECAY,
        ITEM_SPRITEHASH,
        ITEM_MINIMAPCOLOR,
        ITEM_07,
        ITEM_08,
        ITEM_LIGHT,

        //1-byte aligned
        ITEM_DECAY2, //deprecated
        ITEM_WEAPON2, //deprecated
        ITEM_AMU2, //deprecated
        ITEM_ARMOR2, //deprecated
        ITEM_WRITEABLE2, //deprecated
        ITEM_LIGHT2,
        ITEM_TOPORDER,
        ITEM_WRITEABLE3, //deprecated

        ITEM_WAREID,
    }
}