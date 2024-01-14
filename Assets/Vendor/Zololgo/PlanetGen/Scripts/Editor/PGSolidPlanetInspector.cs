using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PGSolidPlanet))]
public class PGSolidPlanetInspector : Editor
{
    bool autoGenerateToggle = false;
    PGSolidPlanet pgSolidPlanet;

    void OnEnable()
    {
        Undo.undoRedoPerformed += OnUndoRedoPerformed;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoRedoPerformed;
    }

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        GUILayout.Space(10);

        autoGenerateToggle = GUILayout.Toggle(autoGenerateToggle, "Auto Generate");

        if (GUILayout.Button("Generate"))
        {
            pgSolidPlanet = target as PGSolidPlanet;
            Undo.RegisterCompleteObjectUndo(pgSolidPlanet, "PG Solid Planet Generate");
            pgSolidPlanet.GeneratePlanet();
        }

        if (GUILayout.Button("Randomize"))
        {
            pgSolidPlanet = target as PGSolidPlanet;
            Undo.RegisterCompleteObjectUndo(pgSolidPlanet, "PG Solid Planet Randomize");
            pgSolidPlanet.RandomizePlanet(true);
        }

        if (GUI.changed)
        {
            pgSolidPlanet = target as PGSolidPlanet;
            Undo.RegisterCompleteObjectUndo(pgSolidPlanet, "PG Solid Planet Inspector change");
            pgSolidPlanet.UpdatePlanetMaterial();
            if (autoGenerateToggle) pgSolidPlanet.GeneratePlanet();
        }
    }

    private void OnUndoRedoPerformed()
    {
        pgSolidPlanet = target as PGSolidPlanet;
        pgSolidPlanet.UpdatePlanetMaterial();
        pgSolidPlanet.GeneratePlanet();
    }
}