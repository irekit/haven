using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D; 
public class main : MonoBehaviour
{
    [SerializeField] private GameObject[] points;
    [SerializeField] private GameObject proxy_body;
    [SerializeField] private Vector3[] initial_positions;
    [SerializeField] private SpriteShapeController ssc;
    [SerializeField] private float move_speed;
    [SerializeField] private GameObject circ;
    private SpriteRenderer circr;
    Vector3 init_pos;
    Rigidbody2D rb;
    private Spline spl;
    LineRenderer lr;
    bool boosted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        circr = circ.GetComponent<SpriteRenderer>();
        circ.GetComponent<CircleCollider2D>().enabled = false;
        init_pos = transform.position;
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
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
            points[i].transform.position = points[i].transform.position - avp + transform.position;
        }
        spl = ssc.spline;
        
    }

    const float des_area = 4f;
    void FixedUpdate()
    {
        Vector3 avp = Vector3.zero;
        float area = 0;
        Vector3[] ps = new Vector3[points.Length];
        bool valid_shape = true;
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
            float mult = 1;
            float tss = Vector3.Dot((points[i].transform.position - transform.position).normalized, initial_positions[i].normalized);
            if (tss < 0)
            {
                mult += Mathf.Pow(1000, -tss);
            }
            if(tss < -0.75f)
            {
                //points[i].transform.position = transform.position + initial_positions[i];
            }
            points[i].GetComponent<Rigidbody2D>().AddForce(0.2f * mult * (initial_positions[i] - points[i].transform.position + transform.position));
            float dea = des_area - area;
            if (dea < 0) dea *= 0.6f;
            //it's like a leaky RELU
            points[i].GetComponent<Rigidbody2D>().AddForce(100f * th.normalized / (th.magnitude * th.magnitude) * dea);

            if (Vector3.Distance(points[next_dex].transform.position, points[i].transform.position) < 0.02f)
            {
                valid_shape = false;
            }
            
        }
        for(int i = 0; i < points.Length; i++)
        {
            //if (valid_shape) spl.SetPosition(i, points[i].transform.position);
        }
        lr.SetPositions(ps);

        //rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.9f, rb.linearVelocity.y);
        //transform.position = avp;
    }
    Vector2 stored_velocity;
    int bounces = 0;
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Respawn"))
        {
            Respawn();
        }
        else
        {
            bounces++;
            boosted = false;
            parryable = true;
        }
    }
    bool charging = false;
    void Respawn()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = init_pos;
        charging = false;
        boosted = false;
        parryable = true;
        bounces = 0;
        for (int i = 0; i < points.Length; i++)
        {
            points[i].transform.position = initial_positions[i] + transform.position;
        }
    }
    bool parryable = true;
    float parry_timer = 0;
    // Update is called once per frame
    void Update()
    {
        if(rb.linearVelocity.magnitude > 50)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * 50;
        }
        proxy_body.transform.position = transform.position;
        if (boosted)
        {
            lr.startColor = Color.red;
        }
        else if (charging)
        {
            lr.startColor = Color.blue;
        }
        else if(bounces > 1)
        {
            lr.startColor = new Color(0, 0, 1, 1);
        }
        else
        {
            lr.startColor = Color.white;
        }
        lr.endColor = lr.startColor;
        Vector2 dir = Vector2.zero;
        if(parry_timer > 0)
        {
            parry_timer -= Time.deltaTime;
            circr.color = new Color(1, 1, 1, parry_timer * 5);
            rb.linearVelocity = Vector2.zero;
            transform.position = charge_pos;
            if(parry_timer <= 0)
            {
                rb.linearVelocity = stored_velocity;
                circ.GetComponent<CircleCollider2D>().enabled = false;
            }
        }
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Respawn();
        }
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
        {
            dir.x++;
        }
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
        {
            dir.x--;
        }
        if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed)
        {
            dir.y++;
        }
        if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed)
        {
            dir.y--;
        }
        dir = dir.normalized;
        if (Keyboard.current.cKey.wasPressedThisFrame && bounces > 1 && parry_timer <= 0)
        {
            stored_velocity = rb.linearVelocity;
            bounces = 0;
            charging = true;
            charge_pos = transform.position;
        }
        if (Keyboard.current.cKey.isPressed && charging)
        {
            rb.linearVelocity = Vector2.zero;
            transform.position = charge_pos;
        }
        if (Keyboard.current.cKey.wasReleasedThisFrame && charging)
        {
            boosted = true;
            charging = false;
            rb.linearVelocity = dir * stored_velocity.magnitude * 1f;
            if(dir == Vector2.zero)
            {
                rb.linearVelocity = stored_velocity;
            }
        }
        if (Keyboard.current.xKey.wasPressedThisFrame && parry_timer <= 0 && parryable && !charging)
        {
            circ.GetComponent<CircleCollider2D>().enabled = true;
            parry_timer = 0.2f;
            parryable = false;
            charge_pos = transform.position;
            stored_velocity = rb.linearVelocity;
        }
    }
    public void Parry()
    {
        //rb.AddForce(Vector2.up * 100, ForceMode2D.Impulse);
        if(stored_velocity.y < 0)
        {
            stored_velocity.y = -stored_velocity.y;
        }
        stored_velocity.y = Mathf.Max(40, stored_velocity.y + 35);
        transform.Translate(0, 1, 0);
        parryable = true;
        bounces = 3;
    }
    Vector3 charge_pos;
}
