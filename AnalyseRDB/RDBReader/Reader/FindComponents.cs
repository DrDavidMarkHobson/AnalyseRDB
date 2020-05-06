using System.Collections.Generic;
using System.Linq;
using RDB.Interface.RDBObjects;

namespace RDBData.Reader
{
    public class FindComponents : IFindComponents
    {
        private string getPinComponentName(string pinName)
        {
            return pinName.Contains("_") ? 
                pinName.Split('_')[0] : 
                string.Empty;
        }

        public IEnumerable<RdbComponent> Find(IEnumerable<RdbNet> nets)
        {
            var allPins = nets.SelectMany(net => net.pins);
            var components = new List<RdbComponent>();

            foreach (var pin in allPins)
            {
                var componentName = getPinComponentName(pin.name);
                var existingComponent =
                    components.FirstOrDefault(component =>
                        component.name == componentName);

                if (existingComponent != null)
                {
                    var pins = existingComponent.pins;
                    pins.Append(pin);
                    existingComponent.pins = pins;
                }
                else
                {
                    if (componentName != null)
                    {
                        components.Add(new RdbComponent
                        {
                            name = componentName,
                            pins = allPins.Where(matchingPin =>
                                getPinComponentName(matchingPin.name) == componentName)
                        });
                    }
                }
            }

            return components;
        }
    }
}