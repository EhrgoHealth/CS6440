using Microsoft.Owin.Security;
using Owin.Security.Providers.Fitbit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhrgoHealth.Web
{
    public class AddAllergyData
    {
        private ApplicationUserManager userManager;
        private IAuthenticationManager authManager;

        public AddAllergyData(ApplicationUserManager userManager, IAuthenticationManager authManager)
        {
            this.userManager = userManager;
            this.authManager = authManager;
        }

        public async Task AddAllergyToMedication(string medicationName, string applicationUserId)
        {
            var user = await userManager.FindByIdAsync(applicationUserId);
            if(!user.AllergicMedications.Any(a => a == medicationName))
            {
                user.AllergicMedications.Add(medicationName);
                await userManager.UpdateAsync(user);
            }
        }

        public async Task RemoveAllergyToMedication(string medicationName, string applicationUserId)
        {
            var user = await userManager.FindByIdAsync(applicationUserId);
            var matches = user.AllergicMedications.Where(a => a == medicationName).ToList();
            for(int i = 0; i < matches.Count; i++)
            {
                user.AllergicMedications.Remove(matches[i]);
            }
        }

        public async Task<List<string>> GetAllergyList(string applicationUserId)
        {
            var user = await userManager.FindByIdAsync(applicationUserId);
            return user.AllergicMedications.ToList();
        }
    }
}