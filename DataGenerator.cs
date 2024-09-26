using Bogus;
using Microsoft.AspNetCore.Routing.Constraints;
using Task_5.Data_Model;

namespace Task_5
{
    public class DataGenerator : Faker<User>
    {
        private static Random Random = new Random();
        private int indexForID = 0;
        private string charsForUs = "1234567890qwertyuiopasdfhjk;zxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM,.";
        private string charsForRu = "1234567890йцунгшщзфывапролдячсмитьЙЦУККЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬ;,.";

        public async Task<List<User>> GenerateUserDataAsync(string region, int seed, int pageNumber, double errorCount, int pageSize = 20)
        {
            var faker = new Faker<User>(region).UseSeed(seed + pageNumber)
                .RuleFor(u => u.Id, (f, u) => GenerateId(seed, pageNumber)).RuleFor(u => u.FullName, f => f.Name.FullName()).RuleFor(u => u.Address, f => f.Address.FullAddress()).RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber());
            var users = faker.Generate(pageSize);
            if (errorCount > 0) { await Task.Run(() => users = ApplyUsersChanges(users, Random, errorCount, region)); }
            return users;
        }

        public List<User> ApplyUsersChanges(List<User> users, Random random, double errorCount, string region)
        {
            foreach (var user in users)
            {
                user.FullName = GetWrongString(user.FullName, random, errorCount, region);
                user.Address = GetWrongString(user.Address, random, errorCount, region);
                user.PhoneNumber = GetWrongString(user.PhoneNumber, random, errorCount, region);
            }
            return users;
        }

        public string GetWrongString(string input, Random random, double errorCount, string region)
        {
            int integerErrorCount = (int)errorCount;
            double fractionPart = errorCount - integerErrorCount;
            if (input.Length > 1) { for (int i = 0; i < integerErrorCount / 3; i++) { if (input.Length <= 1 || input.Length > 150) { break; } else { input = GetErrorType(input, random, region); } } }
            if (random.NextDouble() < fractionPart && input.Length > 1) input = GetErrorType(input, random, region);
            return input;
        }


        public string GetErrorType(string input, Random random, string region)
        {
            if (string.IsNullOrEmpty(input)) return input;
            int errorType = random.Next(1, 4); // 1 - Удаление 1 символа, 2 - добавление 1 случайного символа, 3 - перестановка двух соседних символов местами
            switch (errorType)
            {
                case 1: input = DeleteCharFromStr(input); break;
                case 2: input = input.Insert(random.Next(0, input.Length), GetRandomChar(random, region).ToString()); break;
                case 3: input = SwapChars(input); break;
            }
            return input;
        }
        private string DeleteCharFromStr(string input)
        {
            int i = Random.Next(0, input.Length - 1);
            string res = input.Remove(i, 1);
            return res;
        }

        private string SwapChars(string input)
        {
            int p = Random.Next(0, input.Length - 1);
            char[] chars = input.ToCharArray();
            (chars[p], chars[p + 1]) = (chars[p + 1], chars[p]);
            string res = new string(chars);
            return res;
        }
        private char GetRandomChar(Random random, string region)
        {
            char c = region == "en_US" ? charsForUs[random.Next(0, charsForUs.Length)] : charsForRu[random.Next(0, charsForRu.Length)];
            return c;
        }

        private int GenerateId(int seed, int pageNumber)
        {
            indexForID++;
            int s = seed + pageNumber + indexForID;
            Random random = new Random(s);
            return random.Next(1, 1_000_001);
        }
    }
}
