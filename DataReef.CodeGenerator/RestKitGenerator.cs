using System;
using System.Collections.Generic;
using System.Text;

namespace DataReef.CodeGenerator
{
    public class RestKitGenerator
    {
        public static void WriteObjectCEnumerations(string folderName, List<Entity> entities)
        {
            string headerFileName = System.IO.Path.Combine(folderName, "CoreDataEnums.h");
            System.IO.StreamWriter file = new System.IO.StreamWriter(headerFileName);

            file.WriteLine("//");
            file.WriteLine("//created by DataReef Code Generator");
            file.WriteLine(string.Format("//{0}", System.DateTime.Now.ToString()));
            file.WriteLine("");
            file.WriteLine("");
            file.WriteLine("");

            List<Type> enumTypes = new List<Type>();

            foreach (Entity e in entities)
            {
                foreach(Property p in e.Properties)
                {
                    if (p.Type.IsEnum)
                    {
                        if (!enumTypes.Contains(p.Type)) enumTypes.Add(p.Type);
                    }

                }
            }

            foreach (Type t in enumTypes)
            {
                string[] names = Enum.GetNames(t);
                System.Array values = Enum.GetValues(t);
               
               

                file.WriteLine(string.Format("typedef NS_ENUM(NSInteger, DR{0})",t.Name));
                file.WriteLine("{");

                StringBuilder sb = new StringBuilder();

                int index=0;
                foreach (string name in names)
                {
                    Enum en =(Enum) Enum.Parse(t, name);
                    object val = Convert.ChangeType(en, en.GetTypeCode());

                    sb.AppendLine(string.Format("\tDR{0}{1}={2},", t.Name, name, val));
                    index++; ;
                }

                sb.Remove(sb.Length - 3, 1);

                file.WriteLine(sb.ToString());

                file.WriteLine("};");

                
            }
            file.Close();



        }

        public static void WriteRestKitClasses(string folderName, List<Entity> entities)
        {

            string headerFileName = System.IO.Path.Combine(folderName, "RestKitInitializer.h");
            System.IO.StreamWriter file = new System.IO.StreamWriter(headerFileName);

            file.WriteLine("//");
            file.WriteLine("//created by DataReef Code Generator");
            file.WriteLine(string.Format("//{0}", System.DateTime.Now.ToString()));
            file.WriteLine("");
            file.WriteLine("");
            file.WriteLine(string.Format("@interface RestKitInitializer : NSObject "));
            file.WriteLine("");
            file.WriteLine(string.Format("+(void) initRestkit:(RKObjectManager *)objectManager;"));
            file.WriteLine("");
            file.WriteLine(string.Format("@end"));
            file.Close();


            string implementationFileName = System.IO.Path.Combine(folderName, "RestKitInitializer.m");
            file = new System.IO.StreamWriter(implementationFileName);

            file.WriteLine("//");
            file.WriteLine("//created by DataReef Code Generator");
            file.WriteLine(string.Format("//{0}", System.DateTime.Now.ToString()));
            file.WriteLine("//");
            file.WriteLine("");
            file.WriteLine(string.Format("#import \"RestKitInitializer.h\""));

            foreach (Entity e in entities)
            {
                file.WriteLine(string.Format("#import \"{0}.h\"", e.NamespacePrefix + e.Name));
            }
            file.WriteLine(string.Format("#import <RestKit/RestKit.h>"));

            file.WriteLine();
            file.WriteLine();
            file.WriteLine("@implementation RestKitInitializer");

            file.WriteLine("");
            file.WriteLine("+(void) initRestKit:(RKObjectManager *)objectManager {");
            file.WriteLine("");


            foreach (Entity e in entities)
            {
                file.WriteLine(string.Format("#pragma mark - {0}", e.Name));
                file.WriteLine(string.Format("RKEntityMapping *{1}Mapping = [RKEntityMapping mappingForEntityForName:@\"{0}\" inManagedObjectStore:[RKObjectManager sharedManager].managedObjectStore];", e.NameSpaceAndEntityName, e.Name.ToCocoaCase()));
                file.WriteLine("[" + e.Name.ToCocoaCase() + "Mapping addAttributeMappingsFromDictionary:@{");
                foreach (Property p in e.Properties)
                {
                    file.WriteLine(string.Format("@\"{0}\":@\"{1}\",", p.Name, p.Name.ToCocoaCase()));
                }
                file.WriteLine("}];");
                file.WriteLine(string.Format("{0}Mapping.identificationAttributes=@[@\"guid\"];", e.Name.ToCocoaCase()));
                file.WriteLine("");


                file.WriteLine(string.Format("RKResponseDescriptor *{1}ResponseDescriptor = [RKResponseDescriptor responseDescriptorWithMapping:{1}Mapping method:RKRequestMethodAny pathPattern:@\"/api/v1/0/{0}/:guid\" keyPath:nil statusCodes:RKStatusCodeIndexSetForClass(RKStatusCodeClassSuccessful)];", e.Name.Plural(), e.Name.ToCocoaCase()));
                file.WriteLine(string.Format("[objectManager addResponseDescriptor:{0}ResponseDescriptor];", e.Name.ToCocoaCase()));
                file.WriteLine("");
                file.WriteLine(string.Format("{1}ResponseDescriptor = [RKResponseDescriptor responseDescriptorWithMapping:{1}Mapping method:RKRequestMethodAny pathPattern:@\"/api/v1/0/{0}\" keyPath:nil statusCodes:RKStatusCodeIndexSetForClass(RKStatusCodeClassSuccessful)];", e.Name.Plural(), e.Name.ToCocoaCase()));
                file.WriteLine(string.Format("[objectManager addResponseDescriptor:{0}ResponseDescriptor];", e.Name.ToCocoaCase()));
                file.WriteLine("");
                file.WriteLine(string.Format("RKRequestDescriptor *{0}RequestDescriptor = [RKRequestDescriptor requestDescriptorWithMapping:[{0}Mapping inverseMapping] objectClass:[{1} class] rootKeyPath:nil method:RKRequestMethodAny];", e.Name.ToCocoaCase(), e.NameSpaceAndEntityName));
                file.WriteLine(string.Format("[objectManager addRequestDescriptor:{0}RequestDescriptor];", e.Name.ToCocoaCase()));
                file.WriteLine("");
                file.WriteLine("");

            }

            file.WriteLine("");
            file.WriteLine("");
            file.WriteLine("#pragma mark -");
            file.WriteLine("#pragma mark - Relationships");
            file.WriteLine("");
            file.WriteLine("");



            foreach (Entity e in entities)
            {
                file.WriteLine(string.Format("#pragma mark - {0}", e.Name));
                foreach (Relationship rel in e.Relationships)
                {
                    file.WriteLine(String.Format("[{0}Mapping addPropertyMapping:[RKRelationshipMapping relationshipMappingFromKeyPath:@\"{1}\" toKeyPath:@\"{2}\" withMapping:{3}Mapping]];", e.Name.ToCocoaCase(), rel.Name, rel.Name.ToCocoaCase(), rel.DestinationEntity.ToCocoaCase()));
                    //[groupMapping addPropertyMapping:[RKRelationshipMapping relationshipMappingFromKeyPath:@"Fields" toKeyPath:@"fields" withMapping:fieldMapping]];

                }

                file.WriteLine("");
                file.WriteLine("");

            }

            file.WriteLine(@"}");
            file.WriteLine();
            file.WriteLine(@"@end");
            file.Close();


        }

    }
}
