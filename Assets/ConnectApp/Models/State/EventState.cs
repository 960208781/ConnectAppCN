using System;
using System.Collections.Generic;
using ConnectApp.Models.Model;

namespace ConnectApp.Models.State {
    public enum EventType {
        online,
        offline
    }

    [Serializable]
    public class EventState {
        public bool eventsOngoingLoading { get; set; }
        public bool eventsCompletedLoading { get; set; }
        public List<string> ongoingEvents { get; set; }
        public int ongoingEventTotal { get; set; }
        public List<string> completedEvents { get; set; }
        public int completedEventTotal { get; set; }
        public int pageNumber { get; set; }
        public int completedPageNumber { get; set; }
        public Dictionary<string, IEvent> eventsDict { get; set; }
        public List<IEvent> eventHistory { get; set; }
        public bool eventDetailLoading { get; set; }
        public bool joinEventLoading { get; set; }
        public bool showChatWindow { get; set; }
        public bool openChatWindow { get; set; }
        public string channelId { get; set; }
    }
}