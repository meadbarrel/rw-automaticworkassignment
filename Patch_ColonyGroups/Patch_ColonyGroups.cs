using Lomzie.AutomaticWorkAssignment.PawnPostProcessors;
using Lomzie.AutomaticWorkAssignment.UI;
using Lomzie.AutomaticWorkAssignment.UI.Modular;
using System.Linq;
using TacticalGroups;
using Verse;

namespace Lomzie.AutomaticWorkAssignment.Patches.ColonyGroups
{
    public class Patch_ColonyGroups: Mod
    {
        public Patch_ColonyGroups(ModContentPack content): base(content) {
            PawnSettingUIHandlers.AddHandler(
                new ModularPawnSettingUIHandler<AddToGroupPostProcessor>(
                    new Picker<AddToGroupPostProcessor, ColonistGroup>(
                        (m) => 
                        TacticUtils.AllPawnGroups,
                        x => x.curGroupName,
                        x => x?.group?.curGroupName ?? "AWA.Default".Translate(),
                        (settings, group) => settings.group = group
                    )
                )
            );
            PawnSettingUIHandlers.AddHandler(
                new ModularPawnSettingUIHandler<RemoveFromGroupPostProcessor>(
                    new Picker<RemoveFromGroupPostProcessor, ColonistGroup>(
                        (m) => 
                        TacticUtils.AllPawnGroups,
                        x => x.curGroupName,
                        x => x?.group?.curGroupName ?? "AWA.Default".Translate(),
                        (settings, group) => settings.group = group
                    )
                )
            );
            /*
            PawnSettingUIHandlers.AddHandler(new ModularPawnSettingUIHandler<AddToGroupPostProcessor>(
               new Picker<AddToGroupPostProcessor, ColonistGroup>(
                (m) => 
                    TacticUtils.AllGroups, 
                    x=>x.groupName ?? "AWA.Default".Translate(), 
                    (setting, group) => setting.group=group
                )
            ));
            */
        }
    }

    public class AddToGroupPostProcessor: PawnSetting, IPawnPostProcessor
    {
        public ColonistGroup group;
        private int _groupId;

        public void PostProcess(Pawn pawn, WorkSpecification workSpecification, ResolveWorkRequest resolveWorkRequest)
        {
            if (pawn == null || group == null) return;
            //var groupsFound = TacticUtils.TryGetGroups(pawn, out var groups);
            // if (groupsFound && groups.Contains(group)) return;
            group.Add(pawn);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref group, "Group");

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                _groupId = group.groupID;                
            }
            
            Scribe_Values.Look(ref _groupId, "groupId");

            if (Scribe.mode != LoadSaveMode.Saving)
            {
                group = TacticUtils.AllPawnGroups.Where(_=>_.groupID == _groupId).First();
            }
        }
    }

    public class RemoveFromGroupPostProcessor: PawnSetting, IPawnPostProcessor
    {
        public ColonistGroup group;
        private int _groupId;

        public void PostProcess(Pawn pawn, WorkSpecification workSpecification, ResolveWorkRequest resolveWorkRequest)
        {
            if (pawn == null || group == null) return;
            //var groupsFound = TacticUtils.TryGetGroups(pawn, out var groups);
            // if (groupsFound && groups.Contains(group)) return;
            group.Disband(pawn);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref group, "Group");

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                _groupId = group.groupID;                
            }
            
            Scribe_Values.Look(ref _groupId, "groupId");

            if (Scribe.mode != LoadSaveMode.Saving)
            {
                group = TacticUtils.AllPawnGroups.Where(_=>_.groupID == _groupId).First();
            }
        }
    }
    
    public class RemoveFromAllGroupsPostProcessor: PawnSetting, IPawnPostProcessor
    {
        public void PostProcess(Pawn pawn, WorkSpecification workSpecification, ResolveWorkRequest resolveWorkRequest)
        {
            if (pawn == null) return;
            foreach (var group in TacticUtils.AllPawnGroups)
            {
                group.Disband(pawn);
            }
        }

    }
    
}