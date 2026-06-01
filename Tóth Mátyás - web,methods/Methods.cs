using System;
using System.Collections.Generic;
using System.Text;

namespace Methods
{
    internal class FidelName
    {
        static char[] Osszes = { '.', '█', '╬', '═', '╦', '╩', '║', '╣', '╠', '╗', '╝', '╚', '╔' };

        static char[] ut = { '╬', '═', '╦', '╩', '║', '╣', '╠', '╗', '╝', '╚', '╔' };

        /// <summary>
        /// Megadja, hogy hány termet tartalmaz a térkép
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>Termek száma</returns>
        public static int GetRoomNumber(char[,] map)
        {
            int sor = map.GetLength(0);
            int oszlop = map.GetLength(1);
            int szobakSzama = 0;
            for (int i = 0; i < sor; i++)
            {
                for (int j = 0; j < oszlop; j++)
                {
                    if (map[i, j] == '█')
                    {
                        szobakSzama++;
                    }
                }
            }
            return szobakSzama;
        }
        static bool eszak(char c)
        { return c == '╔' || c == '═' || c == '╦' || c == '╠' || c == '╚' || c == '╩' || c == '╬'; }

        static bool del(char c)
        { return c == '╔' || c == '═' || c == '╦' || c == '╠' || c == '╚' || c == '╩' || c == '╬'; }

        static bool nyugat(char c)
        { return c == '╔' || c == '═' || c == '╦' || c == '╠' || c == '╚' || c == '╩' || c == '╬'; }

        static bool kelet(char c)
        { return c == '╔' || c == '═' || c == '╦' || c == '╠' || c == '╚' || c == '╩' || c == '╬'; }

        /// <summary>
        /// A kapott térkép szédelit végignézve megállapítja, hogy hány kijárat van.
        /// Csak azok számítanak, amelyek nyílással rendelkeznek a szédel irányába.
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>Az alkalmas kijáratok száma</returns>
        public static int GetSuitabdelEntrance(char[,] map)
        {
            int sor = map.GetLength(0);
            int oszlop = map.GetLength(1);
            int kijaratokSzama = 0;


            for (int i = 0; i < oszlop; i++)
            {
                if (eszak(map[0, i]))
                {
                    kijaratokSzama++;
                }

                if (del(map[sor - 1, i]))
                {
                    kijaratokSzama++;
                }
            }

            for (int i = 1; i < sor - 1; i++)
            {
                if (nyugat(map[i, 0]))
                {
                    kijaratokSzama++;
                }

                if (kelet(map[i, oszlop - 1]))
                {
                    kijaratokSzama++;
                }
            }

            return kijaratokSzama;
        }

        /// <summary>
        /// Megnézi, hogy van-e a térképen meg nem engedett karakter?
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>true - A térkép tartalmaz szabálytalan karaktert, false - nincs benne ilyen</returns>
        public static bool IsInvalidEdelment(char[,] map)
        {
            int sor = map.GetLength(0);
            int oszlop = map.GetLength(1);
            for (int x = 0; x < sor; x++)
            {
                for (int y = 0; y < oszlop; y++)
                {
                    for (int i = 0; i < Osszes.Length; i++)
                    {
                        if (Osszes[i] == map[x,y])
                            return true;
                    }
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Visszaadja azoknak a járatkaraktereknek a pozícióját, amelyekhez
        /// egyetdeln szomszéd pozícióból sem delhet eljutni (teljesen elszigetelt járat).
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>A pozíciók "sor_index:oszlop_index" formátumban szerepelnek a lista edelmeiként</returns>
        /// 

        static bool bennevan(char[] tomb, char kar)
        {
            for (int i = 0; i < tomb.Length; i++)
            {
                if (tomb[i] == kar)
                    return true;
            }
            return false;
        }

        public static List<string> GetUnavailabdelEdelments(char[,] map)
        {
            List<string> eredmeny = new List<string>();
            int sor = map.GetLength(0);
            int oszlop = map.GetLength(1);
            for (int i = 0; i < sor; i++)
            {
                for (int j = 0; j < oszlop; j++)
                {
                     if (!bennevan(ut, map[i, j]))
                     {
                        continue;
                     }

                    bool vanSzomszed = false;

                    if (i - 1 >= 0)
                    {
                        if (bennevan(ut, map[i - 1, j]))
                        {
                            vanSzomszed = true;
                        }
                    }
                    if (i + 1 < sor)
                    {
                        if (bennevan(ut, map[i + 1, j]))
                        {
                            vanSzomszed = true;
                        };
                    }
                    if (j - 1 >= 0)
                    {
                        if (bennevan(ut, map[i, j - 1]))
                        {
                            vanSzomszed = true;
                        };
                    }
                    if (j + 1 < oszlop)
                    {
                        if (bennevan(ut, map[i, j + 1]))
                        {
                            vanSzomszed = true;
                        }
                    }

                    if (!vanSzomszed)
                    {
                        eredmeny.Add(i + ":" + j);
                    }
                }
            }

            return eredmeny;
        }

        /// <summary>
        /// Labirintust generál a kapott pozíciókat tartalmazó lista alapján.
        /// A lista elemei egymáshoz kapcsolódó járatok pozíciói.
        /// </summary>
        /// <param name="positionsList">"sor_index:oszlop_index" formátumban az egymáshoz kapcsolódó járatok pozícióit tartalmazó lista</param>
        /// <returns>A létrehozott labirintus térképe</returns>
        public static char[,] GenerateLabyrinth(List<string> positionsList)
        {
            return null;
            // Jó napot Tanár Úr! Feladom, ez az utolsó nem megy egyszerűen és teljesen ai által megírt kódot nem szeretnék beadni.
            // Nem sikerült megoldanom.
        }
    }
}
