namespace BlogMVC.Models {
    public class FollowedModel {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string UserId { get; set; }

        public FollowedModel() { }

        public FollowedModel(string ownerId) {
            OwnerId = ownerId;
        }

    }
}
