using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class PackageController : MonoBehaviour
{
    public List<Package> Packages = new List<Package>();
    public Dictionary<Vector3Int, Package> RestingPackages = new Dictionary<Vector3Int, Package>();
    public GameObject Prefab;
    public GameObject Floor;
    public TextMeshPro HeightLabel;
    public GameObject TallestStackMarker;
    public float Delay = 1;
    public float NextUpdate = 1;
    public TextAsset text;

    private string[] moves;
    private int moveNo;
    private int floorLevel;
    GameObject[] toRemove = new GameObject[81];
    HashSet<Vector3Int> missingFloorTiles = new HashSet<Vector3Int>();
    Vector3Int tallestPoint = new Vector3Int(0, -1, 0);

    public void Start()
    {
        moves = text.text.Split("\n");
        PopulateMissingFloorTiles();
    }

    public void FixedUpdate()
    {
        if (Time.time < NextUpdate) return;        
        NextUpdate += Delay;

        foreach (var package in Packages.ToArray())
        {
            var move = package.GetNextMove();

            var horisontalMovePos = package.IntPos + move.Offset;
            TryMovePackage(package, horisontalMovePos);

            var verticalMovePos = package.IntPos + Move.Down.Offset;
            if (!TryMovePackage(package, verticalMovePos))
            {
                Packages.Remove(package);
                RestingPackages.Add(package.IntPos, package);
                package.Update();
                package.enabled = false;
                CheckFloor(package);
                UpdateTallestStack(package);
            }
        }
        if (moveNo < moves.Length)
        {
            var packageMoves = moves[moveNo];
            var gameObject = Instantiate(Prefab);
            gameObject.transform.parent = transform.parent;
            gameObject.transform.localScale = Vector3.one;
            gameObject.name = "" + moveNo;
            moveNo++;
            var package = gameObject.GetComponent<Package>();
            package.Moves = packageMoves;
            Packages.Add(package);
        }
    }

    private void UpdateTallestStack(Package package)
    {
        if (package.IntPos.y <= tallestPoint.y) return;
        tallestPoint = package.IntPos;
        TallestStackMarker.transform.localPosition = tallestPoint;
        HeightLabel.text = $"Height: {tallestPoint.y * 10 + 10}";
        HeightLabel.gameObject.transform.localPosition = new Vector3(0, tallestPoint.y + 6, 0);
    }

    private bool TryMovePackage(Package package, Vector3Int to)
    {
        if (to.y < 0 ||
            Mathf.Abs(to.x) > 4 ||
            Mathf.Abs(to.z) > 4)
        {
            return false;
        }
        if (RestingPackages.ContainsKey(to)) return false;
        package.IntPos = to;
        return true;
    }

    private void PopulateMissingFloorTiles()
    {
        for (int x = 0; x < 9; x++)
        {
            for (int z = 0; z < 9; z++)
            {
                var vec = new Vector3Int(x - 4, floorLevel, z - 4);
                if (RestingPackages.ContainsKey(vec))
                {
                    toRemove[x * 9 + z] = RestingPackages[vec].gameObject;
                    continue;
                }
                toRemove[x * 9 + z] = null;
                missingFloorTiles.Add(vec);
            }
        }
    }

    private void CheckFloor(Package package)
    {
        var pos = package.IntPos;
        if (missingFloorTiles.Contains(pos))
        {
            toRemove[(pos.x + 4) * 9 + pos.z + 4] = package.gameObject;
            missingFloorTiles.Remove(pos);
        }

        if (missingFloorTiles.Count > 0) return;
        foreach(var obj in toRemove)
        {
            Destroy(obj);
        }
        PopulateMissingFloorTiles();
        floorLevel++;
        Floor.transform.localScale = new Vector3(1, floorLevel, 1);
        var sound = GetComponent<AudioSource>();
        sound.time = 0.5f;
        sound.Play();
    }
}
