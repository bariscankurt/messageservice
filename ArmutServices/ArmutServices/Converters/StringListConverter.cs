using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArmutServices.Serializers
{
    public class StringListConverter
    {
        public List<string> StringToList(string blockedUsersString) 
        {
            //I explained in the below method why i am using null checks so much.
            //As you can see if we don't check null situation it will give us an error
            //when we try to split the null string.
            if (blockedUsersString == null)
            {
                List<string> emptyList = new List<string>();
                return emptyList;
            }
            List<string> blockedUsersList = blockedUsersString.Split(",").ToList();
            return blockedUsersList;
        }
#nullable enable
        public string? ListToString(List<string> blockedUsersList)
        {
            //we need null control because without null check there will be huge
            //empty list problems on the database
            //and of course after that our application will start failing.
            //In this particular method if i don't check "the null situation" first
            //Join method will give us an error. I'm doing this null check to prevent this kind of errors. 
            if (blockedUsersList == null)
                return null;
            string blockedUserString = string.Join(",", blockedUsersList);
            return blockedUserString;
            
        }
    }
}
