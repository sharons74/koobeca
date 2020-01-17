using KoobecaFeedController.BL.Request;

namespace KoobecaFeedController.BL.Handlers
{
    internal class UserLoginHandler : ActivityHandler
    {
        public UserLoginHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            Response = @"
{ 
   ""oauth_token"":""4zulrwyeeabeqiaije5rs968zpak4chd"",
   ""oauth_secret"":""oee9orj2wfxgepd4kmqefbc0f04lycut"",
   ""user"":{
                ""user_id"":3,
      ""email"":""salmon.sharon@gmail.com"",
      ""username"":null,
      ""displayname"":""Sharon Salmon"",
      ""photo_id"":6388,
      ""status"":""Test"",
      ""status_date"":""2019-07-24 14:45:59"",
      ""locale"":""en"",
      ""language"":""en"",
      ""timezone"":""Europe/Moscow"",
      ""search"":1,
      ""show_profileviewers"":1,
      ""level_id"":2,
      ""invites_used"":2,
      ""extra_invites"":0,
      ""enabled"":1,
      ""verified"":1,
      ""approved"":1,
      ""creation_date"":""2018-06-27 08:08:24"",
      ""modified_date"":""2019-07-24 14:45:59"",
      ""lastlogin_date"":""2019-12-05 19:05:16"",
      ""update_date"":null,
      ""member_count"":87,
      ""view_count"":1306,
      ""comment_count"":0,
      ""like_count"":0,
      ""coverphoto"":4923,
      ""coverphotoparams"":""{\""top\"":\""-399\"",\""left\"":\""0\""}"",
      ""view_privacy"":""everyone"",
      ""seao_locationid"":0,
      ""location"":"""",
      ""image"":""http://d18js22wsvyojh.cloudfront.net/public/user/0d/19/5ee203e9151c3d0da899441d0f4b2c2e.jpg"",
      ""image_normal"":""http://d18js22wsvyojh.cloudfront.net/public/user/0f/19/a741cad67833ba7346fa710fdf9d4a78.jpg"",
      ""image_profile"":""http://d18js22wsvyojh.cloudfront.net/public/user/0e/19/9d3b423d15724d0b9fc09aebafc559b1.jpg"",
      ""image_icon"":""http://d18js22wsvyojh.cloudfront.net/public/user/10/19/424a492d509dbe325d11c6f60c65f469.jpg"",
      ""content_url"":""http://beta.koobeca.com/profile/3"",
      ""cover"":""http://d18js22wsvyojh.cloudfront.net/public/user/0d/19/5ee203e9151c3d0da899441d0f4b2c2e.jpg"",
      ""showVerifyIcon"":0
   },
   ""tabs"":{
                ""primemessenger"":""1""
   },
   ""pmAccessToken"":""xt5FcZaFHCpFrFlNmXztqXKDTKLjmZNWoaL06aW69x7pEn0AGHXJzFboqUpwYaaS""
}";
        }
    }
}