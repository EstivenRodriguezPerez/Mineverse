using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float velocidad = 7f;
    public float fuerzaSalto = 12f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D col;

    private float entradaHorizontal;
    private bool enSuelo;
    private bool agachado;

    // Guardar dimensiones originales de la cápsula
    private Vector2 tamanoOriginalCol;
    private Vector2 offsetOriginalCol;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<CapsuleCollider2D>();

        if (col != null)
        {
            tamanoOriginalCol = col.size;
            offsetOriginalCol = col.offset;
        }
    }

    void Update()
    {
        // Detectar si presiona S o Flecha Abajo estando en el suelo
        agachado = (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && enSuelo;

        // Si está agachado, no avanza lateralmente
        if (agachado)
        {
            entradaHorizontal = 0f;
            AjustarCollider(true);
        }
        else
        {
            entradaHorizontal = Input.GetAxisRaw("Horizontal");
            AjustarCollider(false);
        }

        // Voltear sprite según dirección (incluso si gira agachado)
        float direccionVista = Input.GetAxisRaw("Horizontal");
        if (direccionVista > 0)
            spriteRenderer.flipX = false;
        else if (direccionVista < 0)
            spriteRenderer.flipX = true;

        // Salto con Espacio o W (solo si no está agachado)
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && enSuelo && !agachado)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        // Enviar parámetros al Animator
        anim.SetFloat("Velocidad", Mathf.Abs(entradaHorizontal));
        anim.SetBool("EnSuelo", enSuelo);
        anim.SetBool("Agachado", agachado);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(entradaHorizontal * velocidad, rb.linearVelocity.y);
    }

    void AjustarCollider(bool estaAgachado)
    {
        if (col == null) return;

        if (estaAgachado)
        {
            // Reduce la altura de la cápsula y baja el centro para que los pies sigan tocando el suelo
            col.size = new Vector2(tamanoOriginalCol.x, tamanoOriginalCol.y * 0.55f);
            col.offset = new Vector2(offsetOriginalCol.x, offsetOriginalCol.y - (tamanoOriginalCol.y * 0.225f));
        }
        else
        {
            col.size = tamanoOriginalCol;
            col.offset = offsetOriginalCol;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            enSuelo = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            enSuelo = false;
        }
    }
}