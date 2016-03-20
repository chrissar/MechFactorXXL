using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class node {
    // Properties of the node
    private Vector4 mInfluence;
    private float mVisibility = 0;
    private float mProximity = 0;
    private float mSecurity = 0;
    private int mControl = 0;

    public List<node> neighbours;

    public node east;
    public node south;
    public node west;
    public node north;

    public node northeast;
    public node northwest;
    public node southeast;
    public node southwest;

    public node()
    {
        neighbours = new List<node>();
        influence = new Vector4(0, 0, 0, 1);
    }

    public Vector4 influence
    {
        get { return mInfluence; }
        set { mInfluence = value; }
    }

    public float visibility
    {
        get { return mVisibility; }
        set { mVisibility = value; }
    }

    public float proximity
    {
        get { return mProximity; }
        set { mProximity = value; }
    }

    public float security
    {
        get { return mSecurity; }
        set { mSecurity = value; }
    }

    public int control
    {
        get { return mControl; }
        set { mControl = value; }
    }

    public void SetNeighbours()
    {

        if (north != null)
        {
            neighbours.Add(north);
        }

        if (east != null)
        {
            neighbours.Add(east);
        }

        if (south != null)
        {
            neighbours.Add(south);
        }

        if (west != null)
        {
            neighbours.Add(west);
        }

        if (northwest != null)
        {
            neighbours.Add(northwest);
        }

        if (northeast != null)
        {
            neighbours.Add(northeast);
        }

        if (southeast != null)
        {
            neighbours.Add(southeast);
        }

        if (southwest != null)
        {
            neighbours.Add(southwest);
        }
    }
}
