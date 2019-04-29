namespace QueryMaster
{
    /// <summary>
    ///     Specifies the type of engine used by server
    /// </summary>
    public enum EngineType
    {
        /// <summary>
        ///     Source Engine
        /// </summary>
        Source,

        /// <summary>
        ///     Gold Source Engine
        /// </summary>
        GoldSource
    }


    /// <summary>
    ///     Specifies the Region
    /// </summary>
    public enum Region : byte
    {
        /// <summary>
        ///     US East coast
        /// </summary>
        UsEastCoast,

        /// <summary>
        ///     US West coast
        /// </summary>
        UsWestCoast,

        /// <summary>
        ///     South America
        /// </summary>
        SouthAmerica,

        /// <summary>
        ///     Europe
        /// </summary>
        Europe,

        /// <summary>
        ///     Asia
        /// </summary>
        Asia,

        /// <summary>
        ///     Australia
        /// </summary>
        Australia,

        /// <summary>
        ///     Middle East
        /// </summary>
        MiddleEast,

        /// <summary>
        ///     Africa
        /// </summary>
        Africa,

        /// <summary>
        ///     Rest of the world
        /// </summary>
        RestOfTheWorld = 0xFF
    }

    internal enum SocketType
    {
        Udp,
        Tcp
    }

    internal enum ResponseMsgHeader : byte
    {
        A2SInfo = 0x49,
        A2SInfoObsolete = 0x6D,
        A2SPlayer = 0x44,
        A2SRules = 0x45,
        A2SServerqueryGetchallenge = 0x41
    }

    //Used in Source Rcon
    internal enum PacketId
    {
        Empty = 10,
        ExecCmd = 11
    }

    internal enum PacketType
    {
        Auth = 3,
        AuthResponse = 2,
        Exec = 2,
        ExecResponse = 0
    }
}