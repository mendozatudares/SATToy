using System.IO;
using Assets.SAT;
using UnityEngine;

public class Driver : MonoBehaviour
{
    /// <summary>
    /// The Problem object we're trying to solve
    /// </summary>
    private Problem problem;

    /// <summary>
    /// True if the solver should be called to update on every frame.
    /// </summary>
    public bool Run;

    /// <summary>
    /// Number of steps the solver has run for so far.
    /// </summary>
    private int stepCount;

    /// <summary>
    /// Font information for screen display
    /// </summary>
    public GUIStyle GUIStyle;

    /// <summary>
    /// How much blank space to put at the edges of the window
    /// </summary>
    public int Border = 30;
    
    /// <summary>
    /// String buffer to store the noise level the user is typing in the UI
    /// </summary>
    private string noiseLevel = "10";

    /// <summary>
    /// String butter to store the name of the file in the UI
    /// </summary>
    private string fileName = "Test.txt";

    /// <summary>
    /// Called by Unity at the start of the program
    /// </summary>
    public void Start()
    {
        LoadProblem();
    }

    /// <summary>
    /// Load/reload problem from disk
    /// </summary>
    private void LoadProblem()
    {
        problem = new Problem(Path.Combine(Path.Combine(Application.dataPath, "Problems"), fileName));
        stepCount = 0;
        Run = false;
    }

    /// <summary>
    /// Run solver for one step and update GUI.
    /// </summary>
    private bool Step()
    {
        stepCount++;
        var success = problem.StepOne();
        problem.CheckConsistency();
        return success;
    }

    /// <summary>
    /// True if the space bar was just pressed.
    /// </summary>
    private bool SpacePressed => Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space;

    /// <summary>
    /// Called regularly by Unity to redraw the GUI and handle user input.
    /// </summary>
    public void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Border, Border, Screen.width - 2 * Border, Screen.height - 2 * Border));

        // BUTTONS and TEXT BOXES
        GUILayout.BeginHorizontal();
        var runPressed = GUILayout.Button(Run ? "Stop" : "Run", GUIStyle);
        if (runPressed && !Run && problem.IsSolved)
            LoadProblem();
        Run ^= runPressed;
        GUILayout.Space(30);
        if ((GUILayout.Button("Step", GUIStyle) || SpacePressed) && !problem.IsSolved)
            Step();
        GUILayout.Space(30);
        if (GUILayout.Button("Reset", GUIStyle))
            LoadProblem();
        GUILayout.Space(20);
        GUILayout.Label("Noise level: ", GUIStyle);
        noiseLevel = GUILayout.TextField(noiseLevel, GUIStyle);
        if (problem != null && int.TryParse(noiseLevel, out var n))
            problem.NoiseLevel = n;
        GUILayout.Space(20);
        GUILayout.Label("File: ", GUIStyle);
        fileName = GUILayout.TextField(fileName, GUIStyle);
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        // PRINT THE CLAUSES 
        GUILayout.Label("Clauses:", GUIStyle);
        var solved = true;
        if (problem != null)
            foreach (var clause in problem.Clauses)
            {
                var clauseSolved = problem.Satisfied(clause);
                GUILayout.Label(clauseSolved ? $"<color=green>{clause}</color>" : $"<color=red>{clause}</color>",
                    GUIStyle);
                solved &= clauseSolved;
            }

        var maybeSolved = solved ? "<b>solved</b> in " : "";
        GUILayout.Label($"{maybeSolved}{stepCount} steps", GUIStyle);

        GUILayout.Space(20);

        // PRINT THE TRUTH ASSIGNMENT
        GUILayout.Label("Truth assignment", GUIStyle);
        GUILayout.BeginHorizontal();
        if (problem != null)
            foreach (var p in problem.Propositions)
            {
                GUILayout.Label(problem.Solution[p] ? p.Name : $"<color=grey>{p.Name}</color>", GUIStyle);
                GUILayout.Space(20);
            }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        // Do another step if in Run mode.
        if (Event.current.type == EventType.Repaint && Run)
            Run = !Step();
    }
}
