#!/usr/bin/env python3
"""
check_issue_ready.py — Valida que la descripción de un issue cumpla la
Definition of Ready antes de que el loop de desarrollo lo tome.

No se conecta a Jira: el fetch y la actualización del issue (incluyendo
agregar el label "spec-ready") los hace el MCP de Jira dentro del loop
de Claude Code. Este script solo valida el texto de la descripción.

Uso:
    python3 check_issue_ready.py descripcion.md
    cat descripcion.md | python3 check_issue_ready.py -

Salida:
    Código 0 y "LISTO" si cumple la Definition of Ready.
    Código 1 y el detalle de qué falta si no cumple.
"""

import re
import sys

REQUIRED_SECTIONS = [
    "Contexto",
    "Criterio de aceptación",
    "Comando de verificación",
]

VALID_COMMANDS = [
    "dotnet build",
    "dotnet test",
    "ng build",
    "ng test --watch=false",
]

MIN_SECTION_LENGTH = 15  # caracteres, para evitar placeholders vacíos


def find_section(text, section_name):
    """
    Busca una sección tipo '**Contexto**' o 'Contexto:' seguida de
    contenido hasta el siguiente encabezado o el final del texto.
    """
    pattern = (
        rf"(?:\*\*{re.escape(section_name)}\*\*|{re.escape(section_name)})"
        rf"\s*[:\-]?\s*\n?(.+?)"
        rf"(?=\n\s*(?:\*\*[A-ZÁÉÍÓÚÑ]|[A-ZÁÉÍÓÚÑ][a-zA-Záéíóúñ ]+:)|$)"
    )
    match = re.search(pattern, text, re.DOTALL | re.IGNORECASE)
    if not match:
        return None
    return match.group(1).strip()


def check_text(text):
    problems = []

    for section in REQUIRED_SECTIONS:
        content = find_section(text, section)
        if not content or len(content) < MIN_SECTION_LENGTH:
            problems.append(f"- Falta o es muy corta la sección '{section}'")

    verif_content = find_section(text, "Comando de verificación") or ""
    if verif_content and not any(cmd in verif_content for cmd in VALID_COMMANDS):
        problems.append(
            f"- 'Comando de verificación' no contiene ninguno de los comandos "
            f"esperados: {', '.join(VALID_COMMANDS)}"
        )

    return problems


def main():
    if len(sys.argv) != 2:
        print("Uso: python3 check_issue_ready.py <archivo.md>  (o '-' para stdin)")
        sys.exit(2)

    source = sys.argv[1]
    if source == "-":
        text = sys.stdin.read()
    else:
        with open(source, "r", encoding="utf-8") as f:
            text = f.read()

    problems = check_text(text)

    if problems:
        print("NO LISTO\n" + "\n".join(problems))
        sys.exit(1)

    print("LISTO — cumple la Definition of Ready")
    sys.exit(0)


if __name__ == "__main__":
    main()