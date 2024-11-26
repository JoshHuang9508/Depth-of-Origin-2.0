using UnityEngine;
using System.Collections;
using RandomRoom;

public class Room : MonoBehaviour
{
	public RoomType roomType;
	public int index;
}

namespace RandomRoom
{
	public enum RoomType
	{
		general, start, boss
	}
}

