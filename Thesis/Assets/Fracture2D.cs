using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fracture2D : MonoBehaviour {
/*

	GameObject go = new GameObject();
	SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
	Vector2 pivot = new Vector2(csr.sprite.pivot.x / csr.sprite.rect.size.x, csr.sprite.pivot.y / csr.sprite.rect.size.y); // 0-1 range pivot of original sprite.
	Vector2 spriteWorldBounds = csr.sprite.rect.size / csr.sprite.pixelsPerUnit;
	Vector2 spriteWorldBoundsHalfOffset = spriteWorldBounds - (spriteWorldBounds / 2f);
	Vector2 chunkOffset = new Vector2(go.transform.localPosition.x / (spriteWorldBoundsHalfOffset.x), go.transform.localPosition.y / (spriteWorldBoundsHalfOffset.y));
	go.name = chunkOffset.ToString();
	pivot += (chunkOffset / 2f);
	var sprite = Sprite.Create(csr.sprite.texture, csr.sprite.rect, pivot);
	 
	var geo = new InputGeometry();
	foreach (var path in voronoiCell) 
	{
	    foreach(var v in path) 
	    {
	        geo.AddPoint(v.X, v.Y);
	    }
	    for(var i = 0; i < path.Count; i++) {
	        geo.AddSegment(i, (i + 1) % path.Count);
	    }
	}
	 
	var mesh = new TriangleNet.Mesh();
	mesh.Triangulate(geo);
	var vertices = mesh.Vertices.Select(v => ((new Vector2((float)v.X, (float)v.Y) / CLIPPER_SCALE) + (spriteWorldBounds / 2f)) * csr.sprite.pixelsPerUnit).ToArray();
	var triangles = new List<ushort>();
	foreach (var t in mesh.Triangles) 
	{
	    triangles.Add((ushort)t.P0);
	    triangles.Add((ushort)t.P1);
	    triangles.Add((ushort)t.P2);
	}
	sprite.OverrideGeometry(vertices, triangles.ToArray());
	 
	sr.sprite = sprite;
	*/
}
