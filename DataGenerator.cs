using Bogus;
using Microsoft.AspNetCore.Routing.Constraints;
using Task_5.Data_Model;

namespace Task_5
{
    public class DataGenerator : Faker<User>
    {
        private static Random Random = new Random();
        private int indexForID = 0;
        public async Task<List<User>> GenerateUserDataAsync(string region, int seed, int pageNumber, double errorCount, int pageSize = 20)
        {
            var faker = new Faker<User>(region).UseSeed(seed + pageNumber)
                .RuleFor(u => u.Id, (f, u) => GenerateId(seed, pageNumber)).RuleFor(u => u.FullName, f => f.Name.FullName()).RuleFor(u => u.Address, f => f.Address.FullAddress()).RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber());
            var users = faker.Generate(pageSize);
            if (errorCount > 0) { await Task.Run(() => users = ApplyUsersChanges(users, Random, errorCount)); }
            return users;
        }

        public List<User> ApplyUsersChanges(List<User> users, Random random, double errorCount)
        {
            foreach (var user in users)
            {
                user.FullName = GetWrongString(user.FullName, random, errorCount);
                user.Address = GetWrongString(user.Address, random, errorCount);
                user.PhoneNumber = GetWrongString(user.PhoneNumber, random, errorCount);
            }
            return users;
        }

        public string GetWrongString(string input, Random random, double errorCount)
        {
            if (string.IsNullOrEmpty(input)) return input;
            int integerErrorCount = (int)errorCount;
            double fractionPart = errorCount - integerErrorCount;
            for (int i = 0; i < integerErrorCount; i++) { input = GetErrorType(input, random); }
            if (random.NextDouble() < fractionPart) input = GetErrorType(input, random);
            return input;
        }


        public string GetErrorType(string input, Random random)
        {
            if (string.IsNullOrEmpty(input)) return input;
            int errorType = random.Next(1, 4); // 1 - Удаление 1 символа, добавление 1 случайного символа, перестановка двух соседних символвов местами
            switch (errorType)
            {
                case 1: if (input.Length > 0) input = input.Remove(random.Next(0, input.Length)); break;
                case 2: input = input.Insert(random.Next(0, input.Length), GetRandomChar(random).ToString()); break;
                case 3:
                    if (input.Length > 1)
                    {
                        int pos = random.Next(0, input.Length - 1);
                        char[] chars = input.ToCharArray();
                        (chars[pos], chars[pos + 1]) = (chars[pos + 1], chars[pos]);
                        input = new string(chars);
                    }
                    break;
            }
            return input;
        }

        private char GetRandomChar(Random random) { char c = (char)random.Next('a','z'); return c; }

        private int GenerateId(int seed, int pageNumber)
        {
            indexForID++;
            int s = seed + pageNumber + indexForID;
            Random random = new Random(s);
            return random.Next(1, 1_000_001);
        }
    }
}
