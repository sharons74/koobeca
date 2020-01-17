using KoobecaFeedController.BL.Request;

namespace KoobecaFeedController.BL.Handlers
{
    public class MembersSearchHandler : ActivityHandler
    {
        private string _Form;

        public MembersSearchHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
            _Form = @"
{ 
   ""form"":[ 
      {
                ""type"":""Select"",
         ""name"":""profile_type"",
         ""label"":""Member Type"",
         ""multiOptions"":{
                    """":"" "",
            ""1"":""Regular Member""
         }
            },
      {
                ""type"":""Text"",
         ""name"":""displayname"",
         ""label"":""Name""
      },
      {
                ""type"":""Checkbox"",
         ""name"":""has_photo"",
         ""label"":""Only Members With Photos""
      },
      {
                ""type"":""Checkbox"",
         ""name"":""is_online"",
         ""label"":""Only Online Members""
      },
      {
                ""type"":""Submit"",
         ""name"":""submit"",
         ""label"":""Search""
      }
   ],
   ""profile_fields"":{
                ""Personal Information"":[
                   { 
            ""type"":""Text"",
                      ""name"":""1_1_3_alias_first_name"",
                      ""label"":""Name"",
                      ""description"":"""",
                      ""hasValidator"":true
         },
         { 
            ""type"":""Text"",
            ""name"":""1_1_4_alias_last_name"",
            ""label"":""Last Name"",
            ""description"":"""",
            ""hasValidator"":true
         },
         { 
            ""type"":""Date"",
            ""name"":""1_1_6_alias_birthdate"",
            ""label"":""Birthday"",
            ""description"":"""",
            ""hasValidator"":true,
            ""format"":""date""
         }
      ]
   }
}";

        }

        public override void Execute()
        {
            Response = _Form;// "[]";
        }
    }
}