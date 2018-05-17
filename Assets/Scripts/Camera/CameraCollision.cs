using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
	
	public float MinDistance = 1.0f;
	public float MaxDistance = 4.0f;
	public float Smooth = 10.0f;
	public float HitDistance;
	private Vector3 _dollyDir;
	public Vector3 DollyDirAdjusted;
	public float Distance;
	
	void Awake ()
	{
		_dollyDir = transform.localPosition.normalized;
		Distance = transform.localPosition.magnitude;
	}
	
	void Update () 
	{
		Vector3 desiredCameraPos = transform.parent.TransformPoint(_dollyDir * MaxDistance);
		RaycastHit hit;

		if (Physics.Linecast(transform.parent.position, desiredCameraPos, out hit))
		{
			Distance = Mathf.Clamp((hit.distance * HitDistance), MinDistance, MaxDistance);
		}
		else
		{
			Distance = MaxDistance;
		}

		transform.localPosition = Vector3.Lerp(transform.localPosition, _dollyDir * Distance, Time.deltaTime * Smooth);
	}
}
