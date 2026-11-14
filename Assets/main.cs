using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D; 
public class main : MonoBehaviour
{
    [SerializeField] private GameObject[] points;
    [SerializeField] private GameObject proxy_body;
    [SerializeField] private Vector3[] initial_positions;
    [SerializeField] private SpriteShapeController ssc;
    private Spline spl;
    LineRenderer lr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        Vector3 avp = Vector3.zero;
        initial_positions = new Vector3[points.Length];
        for(int i = 0; i < points.Length; i++)
        {
            avp += points[i].transform.position;
        }
        avp /= (float)points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            initial_positions[i] = (points[i].transform.position - avp);
        }
        
    }

    const float des_area = 4f;
    void FixedUpdate()
    {
        Vector3 avp = Vector3.zero;
        float area = 0;
        Vector3[] ps = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            avp += points[i].transform.position;
            int next_dex = (i + 1) % points.Length;
            ps[i] = points[i].transform.position;
            area += points[i].transform.position.x * points[next_dex].transform.position.y - points[next_dex].transform.position.x * points[i].transform.position.y;
        }
        area *= 0.5f;
        area = Mathf.Abs(area);
        Debug.Log("area: " + area);
        avp /= (float)points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            int next_dex = (i + 1) % points.Length;
            int last_dex = (i - 1) % points.Length;
            if (last_dex < 0)
            {
                last_dex += points.Length;
            }
            Vector3 ne = (ps[i] - ps[next_dex]).normalized;
            Vector3 la = (ps[last_dex] - ps[i]).normalized;
            float angie = (Mathf.Atan2(la.y, la.x) * Mathf.Rad2Deg + 90) % 360 * Mathf.Deg2Rad;
            float angie2 = (Mathf.Atan2(ne.y, ne.x) * Mathf.Rad2Deg + 90) % 360 * Mathf.Deg2Rad;
            Vector2 norm = new Vector2(Mathf.Cos(angie) + Mathf.Cos(angie2), Mathf.Sin(angie) + Mathf.Sin(angie2));
            Vector3 th = points[i].transform.position - avp;
            if(Vector3.Dot(norm.normalized, th.normalized) < 0)
            {
                norm = -norm;
            }
            points[i].GetComponent<Rigidbody2D>().AddForce(0.2f * (initial_positions[i] - points[i].transform.position + transform.position));
            float dea = des_area - area;
            if (dea < 0) dea *= 0.6f;
            //it's like a leaky RELU
            points[i].GetComponent<Rigidbody2D>().AddForce(100f * th.normalized / (th.magnitude * th.magnitude) * dea);
        }
        lr.SetPositions(ps);
        //transform.position = avp;
    }
    // Update is called once per frame
    void Update()
    {
        proxy_body.transform.position = transform.position;
        if (Keyboard.current.cKey.isPressed)
        {

        }
    }
}
