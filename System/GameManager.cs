using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverBackground;
    public GameObject winImage;
    public GameObject loseImage;
    public GameObject titleButton;
    public GameObject gameCanvas;

    [SerializeField]
    CameraRig cameraRig;

    [SerializeField]
    NavMeshSurface surface;

    [SerializeField]
    GameObject inputHandlerPrefab;

    Player player;
    InputHandler inputHandler;

    GameObject playerCharacter;
    PlayerData currentPlayer;
    CharacterBaseData bossData;

    GameObject field;

    CharacterControlComponent playerControl;
    ControlComponent bossControl;

    public SliderValueSetter playerHPBar;
    public SliderValueSetter playerMPBar;
    public SliderValueSetter bossHPBar;

    public Image image_qSlotUI;
    public Image image_wSlotUI;
    public Image image_eSlotUI;

    public SkillIconCooldownSetter qCoolSetter;
    public SkillIconCooldownSetter wCoolSetter;
    public SkillIconCooldownSetter eCoolSetter;
    public DashIconCooldownSetter dashCoolSetter;

    bool isGameOver = false;

    void Start()
    {
        currentPlayer = LoginDataManager.Instance.currentPlayer;
        int bossIndex = currentPlayer.recentBoss;
        int charIndex = currentPlayer.recentCharacter;

        bossData = CharacterDatabase.Instance.bosses[bossIndex];
        var charData = CharacterDatabase.Instance.players[charIndex];

        field = Instantiate(bossData.mapPrefab);
        surface.BuildNavMesh();

        //=================================================

        player = new Player();
        inputHandler = Instantiate(inputHandlerPrefab).GetComponent<InputHandler>();

        playerCharacter = Instantiate(charData.prefab, field.transform.Find("PlayerStartPositions").GetChild(0).position, Quaternion.identity);
        var control = playerCharacter.GetComponent<CharacterControlComponent>();
        player.character = control;
        playerControl = player.character;
        player.inputHandler = inputHandler;
        inputHandler.player = player;
        
        cameraRig.transform.position = playerCharacter.transform.position;
        cameraRig.target = playerCharacter.transform;

        Stats playerStats = playerCharacter.GetComponent<Stats>();
        playerStats.InitializeStats();
        playerHPBar.targetStats = playerStats;
        playerHPBar.targetType = Stat.Type.MaxHP;
        playerMPBar.targetStats = playerStats;
        playerMPBar.targetType = Stat.Type.MaxMP;

        SkillSlots slots = playerControl.skillSlots;
        qCoolSetter.slot = slots.q;
        if (slots.q.skill is null)
        {
            image_qSlotUI.sprite = null;
        }
        else
        {
            image_qSlotUI.sprite = slots.q.skill.sprite;
        }
        wCoolSetter.slot = slots.w;
        if (slots.w.skill is null)
        {
            image_wSlotUI.sprite = null;
        }
        else
        {
            image_wSlotUI.sprite = slots.w.skill.sprite;
        }
        eCoolSetter.slot = slots.e;
        if (slots.e.skill is null)
        {
            image_eSlotUI.sprite = null;
        }
        else
        {
            image_eSlotUI.sprite = slots.e.skill.sprite;
        }
        dashCoolSetter.move = playerControl.movement;

        ItemSlots items = playerCharacter.GetComponent<ItemSlots>();
        items.AssignItem();

        var settings = currentPlayer.dic_charSettings[currentPlayer.recentCharacter];
        if (settings.weapon != -1)
        {
            var weaponEffectPrefabs = ItemDatabase.Instance.weapons[settings.weapon].effectsPrefabs;
            foreach (var prefab in weaponEffectPrefabs)
            {
                var weapon = Instantiate(prefab).GetComponent<ItemBase>();
                weapon.owner = playerCharacter;
                weapon.GetOn();
            }
        }
        
        if (settings.armor != -1)
        {
            var armorEffectPrefabs = ItemDatabase.Instance.armors[settings.armor].effectsPrefabs;
            foreach (var prefab in armorEffectPrefabs)
            {
                var armor = Instantiate(prefab).GetComponent<ItemBase>();
                armor.owner = playerCharacter;
                armor.GetOn();
            }
        }
        
        if (settings.head != -1)
        {
            var helmetEffectPrefabs = ItemDatabase.Instance.helmets[settings.head].effectsPrefabs;
            foreach (var prefab in helmetEffectPrefabs)
            {
                var helmet = Instantiate(prefab).GetComponent<ItemBase>();
                helmet.owner = playerCharacter;
                helmet.GetOn();
            }
        }

        playerStats.enabled = true;
        slots.enabled = true;
        items.enabled = true;
        playerCharacter.GetComponent<NavMeshAgent>().enabled = true;
        playerCharacter.GetComponent<Movement>().enabled = true;
        playerCharacter.GetComponent<ItemSlots>().enabled = true;
        control.enabled = true;

        inputHandler.isReady = true;
        //===============================================

        bossControl = Instantiate(bossData.prefab, field.transform.Find("BossStartPositions").GetChild(0).position, Quaternion.LookRotation(Vector3.back)).GetComponent<ControlComponent>();
        bossControl.mapCenter = field.transform.Find("SpecialPositions").GetChild(0).transform.position;

        Stats bossStats = bossControl.GetComponent<Stats>();
        bossStats.InitializeStats();
        bossHPBar.targetStats = bossStats;
        bossHPBar.targetType = Stat.Type.MaxHP;

        gameCanvas.SetActive(true);
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }

        if (bossControl.isDead)
        {
            foreach (var mat in bossData.rewards) 
            {
                if (currentPlayer.dic_itemMaterials.ContainsKey(mat))
                {
                    currentPlayer.dic_itemMaterials[mat] += 1;
                }
                else
                {
                    currentPlayer.dic_itemMaterials[mat] = 1;
                }
            }

            playerControl.isEnd = true;

            gameCanvas.SetActive(false);

            //铰府贸府
            gameOverBackground.SetActive(true);
            winImage.SetActive(true);
            titleButton.SetActive(true);

            isGameOver = true;
        }
        else if (playerControl.isDead)
        {
            bossControl.isEnd = true;

            gameCanvas.SetActive(false);

            //菩硅贸府
            gameOverBackground.SetActive(true);
            loseImage.SetActive(true);
            titleButton.SetActive(true);

            isGameOver = true;
        }
    }

    public void GoTitleScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
