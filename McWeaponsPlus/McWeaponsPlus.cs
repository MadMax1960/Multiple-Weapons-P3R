using p3rpc.commonmodutils;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Memory;
using Reloaded.Memory.Structs;

namespace McWeaponsPlus
{
    public class McWeaponsPlus : ModuleAsmInlineColorEdit<WeaponContext>
    {
        private string UEquip_McWeaponsPlus_SIG = "0F A3 C8 73 ?? B0 01 48 8B 5C 24 ??";
        private string UShop_SaveSelectedEquipment = "41 8B D8 E8 ?? ?? ?? ?? 4C 8B 85 ?? ?? ?? ??";
        private string UShop_ResetSelectedEquipment = "4C 8B 6C 24 ?? E8 ?? ?? ?? ?? 84 C0";

        private IAsmHook _McWeaponsPlus;
        private IAsmHook _SaveSelectedEquipment;
        private IAsmHook _ResetSelectedEquipment;

        private MemoryAllocation _equipmentType;

        public McWeaponsPlus(WeaponContext context, Dictionary<string, ModuleBase<WeaponContext>> modules) : base(context, modules)
        {
            _equipmentType = Memory.Instance.Allocate(sizeof(int)); // Allocate equipmentType memory and assign it with -1
            Memory.Instance.WriteWithMarshalling(_equipmentType.Address, -1);

            _context._utils.SigScan(UEquip_McWeaponsPlus_SIG, "UEquip::McWeaponsPlus", _context._utils.GetDirectAddress, addr =>
            {
                string[] function =
                {
                    "use64",

                    $"mov rbx, 0x{_equipmentType.Address:X}",
                    "mov edi, [rbx]",
                    "cmp edi, -0x1",
                    "je .checkWeaponId", // Check whether we are in shop or not

                    "cmp edi, 0x1", // Compare equipment type with 1 (weapons)
                    "jne .original", // If we are not checking weapons in the shop, skip custom code
                    "jmp .checkCharacterIds", // Else go to character id checking

                    ".checkWeaponId:",
                    "cmp esi, 0x1FF",
                    "jg .original", // If we are not equiping weapons, skip custom code

                    ".checkCharacterIds:",
                    "cmp ecx, 0x1",
                    "jne .original", // If we are not MC, skip custom code

                    "cmp eax, 0x80",
                    "je .original", // If weapon has Aigis equip id, skip custom code

                    "mov eax, 0x2", // If we reach here, this forces Makoto's equip id

                    ".original:"
                };

                _McWeaponsPlus = _context._hooks.CreateAsmHook(function, addr, AsmHookBehaviour.ExecuteFirst).Activate();
            });

            _context._utils.SigScan(UShop_SaveSelectedEquipment, "UShop::SaveSelectedEquipment", _context._utils.GetDirectAddress, addr =>
            {
                string[] function =
                {
                    "use64",
                    $"mov rbx, 0x{_equipmentType.Address:X}", // Save shop equipment type as global to check in equipment function later
                    "mov [rbx], r8d"
                };

                _SaveSelectedEquipment = _context._hooks.CreateAsmHook(function, addr, AsmHookBehaviour.ExecuteFirst).Activate();
            });

            _context._utils.SigScan(UShop_ResetSelectedEquipment, "UShop::ResetSelectedEquipment", _context._utils.GetDirectAddress, addr =>
            {
                string[] function =
                {
                    "use64",
                    $"mov r13, 0x{_equipmentType.Address:X}", // Reset shop equipment type to check in equipment function later
                    "mov dword [r13], -0x1"
                };

                _ResetSelectedEquipment = _context._hooks.CreateAsmHook(function, addr, AsmHookBehaviour.ExecuteFirst).Activate();
            });
        }

        ~McWeaponsPlus()
        {
            Memory.Instance.Free(_equipmentType);
        }

        public override void Register()
        {
        }
    }
}
