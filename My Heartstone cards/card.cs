using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace My_Heartstone_cards
{
   public class card
    {

        public static List<card> cardList = new List<card>();

        public string CardId { get; set; }
        public string Name { get; set; }
        public string CardSet { get; set; }
        public string Type { get; set; }
        public string Faction { get; set; }
        public string Rarity { get; set; }
        public string Cost { get; set; }
        public string Attack { get; set; }
        public string Health { get; set; }
        public string Text { get; set; }
        public string Flavor { get; set; }
        public string Artist { get; set; }
        public string HowToGetGold { get; set; }
        public string Img { get; set; }
        public string ImgGold { get; set; }
        //public mecanics[] Mechanics { get; set; }

        /*"cardId": "CS1_042",
      "name": "Goldshire Footman",
      "cardSet": "Basic",
      "type": "Minion",
      "faction": "Alliance",
      "rarity": "Common",
      "cost": 1,
      "attack": 1,
      "health": 2,
      "text": "<b>Taunt</b>",
      "flavor": "If 1/2 minions are all that is defending Goldshire, you would think it would have been overrun years ago.",
      "artist": "Donato Giancola",
      "collectible": true,
      "howToGetGold": "Unlocked at Paladin Level 57.",
      "img": "http://wow.zamimg.com/images/hearthstone/cards/enus/original/CS1_042.png",
      "imgGold": "http://wow.zamimg.com/images/hearthstone/cards/enus/animated/CS1_042_premium.gif",
      "locale": "enUS",
      "mechanics": [
        {
          "name": "Taunt"
        }
      ]*/
    }
}
