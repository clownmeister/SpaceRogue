﻿using UnityEngine;

namespace SpaceRogue.Map
{
    public static class MapNodeNameGenerator
    {
        private static readonly string[] EmptyZonePrefixes =
        {
            "Silent", "Void", "Null", "Desolate", "Barren",
            "Empty", "Obscure", "Forsaken", "Ghost", "Abyssal",
            "Cold", "Dark", "Eclipsed", "Isolated", "Vacant",
            "Shadow", "Cosmic", "Uncharted", "Forgotten", "Lost"
        };

        private static readonly string[] EmptyZoneSuffixes =
        {
            "Expanse", "Field", "Sector", "Reach", "Quarter",
            "Basin", "Realm", "Zone", "Plain", "Void",
            "Nexus", "Domain", "Rift", "Nebula", "Gulf",
            "Space", "Depths", "Haven", "Abyss", "Oasis"
        };
        private static readonly string[] PlanetPrefixes =
        {
            "Terra", "Neo", "Zeta", "Aqua", "Pyro",
            "Gaia", "Helios", "Atlas", "Eos", "Cronus",
            "Artemis", "Dione", "Janus", "Luna", "Nova",
            "Orion", "Pegasus", "Quasar", "Rhea", "Selene"
        };

        private static readonly string[] PlanetSuffixes =
        {
            "Prime", "Secundus", "III", "IV", "V",
            "VI", "VII", "VIII", "IX", "X",
            "Major", "Minor", "Alpha", "Beta", "Gamma",
            "Delta", "Epsilon", "Zeta", "Eta", "Theta"
        };

        private static readonly string[] AsteroidPrefixes =
        {
            "Asteroid", "Rocky", "Metallic", "Dusty", "Icy",
            "Shadow", "Cosmic", "Glimmer", "Hollow", "Iron",
            "Jagged", "Krypton", "Luminous", "Magnetic", "Nebula",
            "Obsidian", "Palladium", "Quartz", "Radiant", "Silica"
        };
        private static readonly string[] AsteroidSuffixes =
        {
            "Belt", "Field", "Cluster", "Ring", "Trail",
            "Zone", "Expanse", "Void", "Formation", "Realm",
            "Domain", "Sector", "Grid", "Labyrinth", "Orbit",
            "Pathway", "Quadrant", "Region", "Territory", "Vortex"
        };

        private static readonly string[] BlackHoleNames =
        {
            "Eventide", "Singularity", "Vortex", "Eclipse", "Nebulor",
            "Oblivion", "Voidheart", "Darkstar", "Abyss", "Phantom",
            "Inferno", "Maelstrom", "Twilight", "Shadowfrost", "Requiem",
            "Eternity", "Whisper", "Nightfall", "Horizon", "Gravity's End"
        };

        private static string[] _starNames =
        {
            "Sol", "Proxima", "Vega", "Sirius", "Rigel",
            "Altair", "Antares", "Betelgeuse", "Canopus", "Deneb",
            "Eridanus", "Fomalhaut", "Gacrux", "Hadar", "Izar",
            "Kochab", "Larawag", "Mimosa", "Nunki", "Polaris"
        };

        public static string GenerateName(MapNodeType nodeType)
        {
            return nodeType switch
            {
                MapNodeType.Planet => $"{PlanetPrefixes[Random.Range(0, PlanetPrefixes.Length)]} {PlanetSuffixes[Random.Range(0, PlanetSuffixes.Length)]}",
                MapNodeType.Asteroids => $"{AsteroidPrefixes[Random.Range(0, AsteroidPrefixes.Length)]} {AsteroidSuffixes[Random.Range(0, AsteroidSuffixes.Length)]}",
                MapNodeType.BlackHole => BlackHoleNames[Random.Range(0, BlackHoleNames.Length)],
                MapNodeType.Empty => $"{EmptyZonePrefixes[Random.Range(0, EmptyZonePrefixes.Length)]} {EmptyZoneSuffixes[Random.Range(0, EmptyZoneSuffixes.Length)]}",
                _ => "Unknown"
            };
        }
    }
}