using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeusCloud.Data.Entities.GameEvents
{
    public class GameEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Index]
        [StringLength(250)]
        public string User { get; protected set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public GameEventType Type { get; set; }

        public GameEvent()
        {
        }

        public GameEvent(GameEventType t, string user)
        {
            Time = DateTime.Now;
            Type = t;
            User = user;
        }

        public string Description { get; set; }
    }
}