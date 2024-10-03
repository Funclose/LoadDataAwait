using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace LoadDataUser
{
     class Test
    {
         public async Task GetArray(string[] name)
        {
            await Task.Run(() =>
            {
                var dublicate = name.GroupBy(n => n).Where(c => c.Count() > 1).Select(c => c.Key).ToList();
                if (dublicate.Count() > 0)
                {
                    throw new OperationCanceledException($"ошибка вывода, дублируеться: {string.Join(",", dublicate)}");
                }
                else
                {
                    Console.WriteLine($"Имена: {string.Join(", ", name)}");
                }
            });
        }
    }
}
