using System.ComponentModel.DataAnnotations;

namespace Mendi.Blazor.DynamicNavigation
{
    public enum StorageUtilityType
    {
        [Display(Description = "Browser Local Storage - Edittable")]
        LocalStorage,
        [Display(Description = "Browser's Index BD - Non Editable")]
        IndexDb
    }
}
