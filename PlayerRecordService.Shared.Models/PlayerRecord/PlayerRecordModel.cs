using SkycavePlayerService.Shared.Models.PlayerRecord.Primitives;

namespace SkycavePlayerService.Shared.Models.PlayerRecord
{
    /// <summary>
    ///  The PlayerRecordModel is the aggregate root for the player records.
    ///  It ensures the integrity of data enroute from the API(Service layer) to the connector implementations(Data source layer) and vice versa,
    ///  Ensuring that data transfer objects and similar (database object) are decoupled from each other and just and importantly, from the intermediate
    ///  layers between the service layer and data source layer, so the application domain logic have are not couples to external contracts.
    /// https://martinfowler.com/eaaCatalog/serviceLayer.html
    /// </summary>
    public class PlayerRecordModel
    {
        private readonly PlayerId playerId;
        private readonly PlayerName playerName;
        private readonly GroupName groupName;
        private readonly Region region;
        private readonly Position position;
        private readonly AccessToken accessToken;

        

        public PlayerRecordModel(
            string playerId, string playerName, string groupName, Region region, string position, string accessToken)
        {

            this.playerId = new PlayerId(playerId);
            this.playerName = new PlayerName(playerName);
            this.groupName = new GroupName(groupName);
            this.region = region;
            this.position = new Position(position);
            this.accessToken = new AccessToken(accessToken);
        }

        public PlayerId PlayerId => playerId;
        public PlayerName PlayerName => playerName;
        public GroupName GroupName => groupName;
        public Region Region => region;
        public Position Position => position;
        public AccessToken AccessToken => accessToken;

    }
    public enum Region
    {
        AARHUS,
        COPENHAGEN,
        ODENSE,
        AALBORG
    }
}