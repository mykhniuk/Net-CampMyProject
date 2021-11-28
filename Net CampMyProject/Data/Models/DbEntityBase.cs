using System;
using System.ComponentModel.DataAnnotations;

namespace Net_CampMyProject.Data.Models
{
    public abstract class DbEntityBase<T> where T : IComparable
    {
        [Key]
        public T Id { get; set; }
    }
}