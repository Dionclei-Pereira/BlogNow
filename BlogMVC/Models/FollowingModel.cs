namespace BlogMVC.Models {
    public class FollowingModel {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string UserId { get; set; }

        public FollowingModel() { }

        public FollowingModel(string ownerId) {
            OwnerId = ownerId;
        }

    }
}
