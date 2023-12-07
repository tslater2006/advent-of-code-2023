using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Day07 : BaseDay
    {
        enum HandType : int
        {
            FiveOfAKind = 6,
            FourOfAKind = 5,
            FullHouse = 4,
            ThreeOfAKind = 3,
            TwoPair = 2,
            OnePair = 1,
            HighCard = 0
        }

        enum Card : byte
        {
            Joker = 0,
            Two = 1,
            Three = 2,
            Four = 3,
            Five = 4,
            Six = 5,
            Seven = 6,
            Eight = 7,
            Nine = 8,
            Ten = 9,
            Jack = 10,
            Queen = 11,
            King = 12,
            Ace = 13
        }

        class Hand : IComparable<Hand>
        {
            public bool UseJokers = false;
            public string CardString;
            public Card[] Cards = new Card[5];
            public int Bid;

            public HandType GetHandType()
            {
                int pairCounts = 0;
                int jokerCounts = 0;
                int pairsWithJoker = 0;
                byte[] cardCounts = new byte[14];
                foreach(var card in Cards)
                {
                    if (UseJokers && card == Card.Jack)
                    {
                        jokerCounts++;
                        continue;
                    }
                    var cardIndex = (int)card;
                    cardCounts[cardIndex]++;           
                }
                HandType bestHandType = HandType.HighCard;

                foreach (var count in cardCounts.OrderByDescending(c => c))
                {
                    if (count == 5 || count + jokerCounts == 5)
                    {
                        bestHandType = HandType.FiveOfAKind;
                    } else if (count == 4 || count + jokerCounts == 4 && bestHandType < HandType.FourOfAKind)
                    {
                        bestHandType = HandType.FourOfAKind;
                    } else if (count == 3 || count + jokerCounts == 3 && bestHandType < HandType.ThreeOfAKind)
                    {
                        bestHandType = HandType.ThreeOfAKind;
                    }else if (count == 2 || (count + jokerCounts - pairsWithJoker) == 2 && bestHandType < HandType.TwoPair)
                    {
                        pairCounts++;
                        if (jokerCounts > 0)
                        {
                            pairsWithJoker++;
                        }
                        if (pairCounts == 2 && bestHandType < HandType.TwoPair)
                        {
                            bestHandType = HandType.TwoPair;
                        } else if (pairCounts == 1 && bestHandType < HandType.OnePair)
                        {
                            bestHandType = HandType.OnePair;
                        }
                    }

                    if (count == 0) { break; }
                }

                if (bestHandType == HandType.ThreeOfAKind && pairCounts == 1)
                {
                    return HandType.FullHouse;
                }

                return bestHandType;
            }

            public int CompareTo(Hand other)
            {
                var myType = GetHandType();
                var otherType = other.GetHandType();

                if (myType != otherType)
                {
                    return myType.CompareTo(otherType);
                } else
                {
                    /* Compare cards in order, first to have a high card wins */
                    for (var x = 0; x < 5; x++)
                    {
                        var myCard = Cards[x] == Card.Jack && UseJokers ? Card.Joker : Cards[x];
                        var theirCard = other.Cards[x] == Card.Jack && UseJokers ? Card.Joker : other.Cards[x];
                        if (myCard != theirCard)
                        {
                            return myCard.CompareTo(theirCard);
                        }
                    }
                }
                return 0;
            }
        }

       

        List<Hand> Hands = new();
        public Day07()
        {
            var lines = File.ReadAllLines(InputFilePath);
            foreach (var line in lines)
            {
                var parts = line.Split(' ');
                var hand = new Hand();
                Hands.Add(hand);
                var cards = parts[0];
                hand.Bid = int.Parse(parts[1]);
                hand.CardString = parts[0];
                for (var x = 0; x < cards.Length; x++)
                {
                    if (cards[x] >= '2' && cards[x] <= '9')
                    {
                        hand.Cards[x] = (Card)(cards[x] - '1');
                    }
                    else
                    {
                        switch (cards[x])
                        {
                            case 'T':
                                hand.Cards[x] = Card.Ten;
                                break;
                            case 'J':
                                hand.Cards[x] = Card.Jack;
                                break;
                            case 'Q':
                                hand.Cards[x] = Card.Queen;
                                break;
                            case 'K':
                                hand.Cards[x] = Card.King;
                                break;
                            case 'A':
                                hand.Cards[x] = Card.Ace;
                                break;
                        }
                    }
                }

            }
        }
        public override ValueTask<string> Solve_1()
        {
            Hands.Sort();
            long winnings = 0;
            for(var x = 0; x < Hands.Count; x++)
            {
                winnings += Hands[x].Bid * (x + 1);
            }

            return new(winnings.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            foreach(var h in Hands)
            {
                h.UseJokers = true;
            }
            Hands.Sort();
            var f = File.CreateText("debug.txt");
            long winnings = 0;
            for (var x = 0; x < Hands.Count; x++)
            {
                f.WriteLine($"{Hands[x].CardString} {Hands[x].GetHandType()}");
                winnings += Hands[x].Bid * (x + 1);
            }
            f.Close();
            return new(winnings.ToString());
        }
    }
}
