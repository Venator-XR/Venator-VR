using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors; // Namespace del NearFarInteractor
using UnityEngine.XR.Interaction.Toolkit;
using System.Reflection;

public static class XRInteractorExtensions
{
    /// <summary>
    /// Método de extensión: Añade funcionalidad extra a NearFarInteractor sin tocar su código original.
    /// Uso: tuInteractor.ForceSetGripType(2);
    /// </summary>
    public static void ForceSetGripType(this NearFarInteractor interactor, int modeIndex)
    {
        if (interactor == null) return;

        // Buscamos la variable en la clase BASE (XRBaseInputInteractor) que es donde vive realmente
        // Usamos Reflection para acceder aunque sea privada/protegida
        var type = typeof(XRBaseInputInteractor);
        
        // Buscamos el campo 'm_SelectActionTrigger'
        var field = type.GetField("m_SelectActionTrigger", 
            BindingFlags.NonPublic | 
            BindingFlags.Instance);

        if (field != null)
        {
            // Inyectamos el valor en la instancia que nos han pasado
            field.SetValue(interactor, modeIndex);
            // Debug.Log($"[Extension] Grip Mode cambiado a {modeIndex} en {interactor.name}");
        }
        else
        {
            // Plan B: Intentar buscar por propiedad pública si existiera en el futuro
            var prop = type.GetProperty("selectActionTrigger", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(interactor, modeIndex);
            }
            else
            {
                Debug.LogWarning("No se pudo hackear el SelectActionTrigger. Puede que Unity haya cambiado el nombre interno de la variable.");
            }
        }
    }
}