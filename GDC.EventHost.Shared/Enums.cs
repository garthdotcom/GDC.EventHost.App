using System.Text.Json.Serialization;

namespace GDC.EventHost.Shared
{
    public class Enums
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SeatTypeEnum { 
            Standard = 0, 
            Premium = 1, 
            Standing = 2, 
            Wheelchair = 3, 
            GeneralAdmission = 4 
        };

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SeatPositionTypeEnum { 
            Section = 0, 
            Row = 1, 
            Stand = 2, 
            Box = 3, 
            Balcony = 4, 
            Aisle = 5 
        };

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum StatusEnum { 
            Active = 0, 
            Pending = 1, 
            Deleted = 2 
        };

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum TicketStatusEnum { 
            UnSold = 0, 
            SalePending = 1, 
            Sold = 2,
            Reserved = 3
        };

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum AssetTypeEnum { 
            SmallImage = 0, 
            MediumImage = 1, 
            LargeImage = 2,
            TinyImage = 3,
            Avatar = 4
        };

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum OrderStatusEnum
        { 
            Incomplete = 0,
            Processing = 1,
            Complete = 2,
            Cancelled = 3,
            Rejected = 4
        };
    }
}
